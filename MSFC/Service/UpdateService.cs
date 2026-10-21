using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MSFC.Dto;

namespace MSFC.Service
{
    public class UpdateService
    {
        private readonly HttpClient _httpClient;

        public UpdateService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<UpdateCheckResponse?> CheckForUpdatesAsync(string serverUrl, string checkApi, string currentVersion)
        {
            try
            {
                serverUrl = serverUrl.TrimEnd('/');
                string requestUrl;
                if (checkApi.Contains("{0}"))
                {
                    requestUrl = serverUrl + string.Format(checkApi, currentVersion);
                }
                else
                {
                    requestUrl = serverUrl + checkApi + (checkApi.Contains("?") ? "&" : "?") + "currentVersion=" + currentVersion;
                }

                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<UpdateCheckResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null && !string.IsNullOrEmpty(result.DownloadUrl) && !result.DownloadUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    result.DownloadUrl = serverUrl + (result.DownloadUrl.StartsWith("/") ? "" : "/") + result.DownloadUrl;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking updates: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadFileWithProgressAsync(string downloadUrl, string destinationPath, IProgress<int>? progress, CancellationToken ct)
        {
            using var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            long totalBytes = response.Content.Headers.ContentLength ?? -1L;
            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, ct);
                totalRead += bytesRead;

                if (totalBytes > 0 && progress != null)
                {
                    int percentage = (int)((totalRead * 100) / totalBytes);
                    progress.Report(percentage);
                }
            }
        }

        public bool VerifySha256(string filePath, string expectedHash)
        {
            if (string.IsNullOrWhiteSpace(expectedHash) || !File.Exists(filePath))
                return true; // Nếu server không trả mã sha256 thì bỏ qua kiểm tra hash

            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            byte[] hash = sha256.ComputeHash(stream);
            string actualHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return string.Equals(actualHash, expectedHash.Trim().ToLowerInvariant(), StringComparison.OrdinalIgnoreCase);
        }

        public void ExecuteUpdatePackage(string zipPath, string newVersion)
        {
            // Update appsettings.json right now before exiting
            if (!string.IsNullOrWhiteSpace(newVersion))
            {
                try
                {
                    AppConfigManager.UpdateSetting("AppUpdate", "CurrentVersion", newVersion);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating config before exit: {ex.Message}");
                }
            }

            string appDir = AppContext.BaseDirectory.TrimEnd('\\');
            string tempDir = Path.Combine(Path.GetTempPath(), "MSFC_Update");
            Directory.CreateDirectory(tempDir);

            string batchPath = Path.Combine(tempDir, "run_update.bat");
            int currentPid = Environment.ProcessId;
            string mainExeName = Process.GetCurrentProcess().MainModule?.FileName ?? Path.Combine(appDir, "MSFC.exe");
            string appSettingsPath = Path.Combine(appDir, "appsettings.json");

            // Build batch script to wait for process exit, extract zip over app dir, and ensure appsettings.json has newVersion
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("@echo off");
            sb.AppendLine("chcp 65001 > nul");
            sb.AppendLine("echo Waiting for MSFC.exe to exit...");
            sb.AppendLine($"taskkill /F /PID {currentPid} > nul 2>&1");
            sb.AppendLine("timeout /t 2 /nobreak > nul");
            sb.AppendLine($"echo Extracting update package from {zipPath}...");
            sb.AppendLine($"powershell -Command \"Expand-Archive -Path '{zipPath}' -DestinationPath '{appDir}' -Force\"");
            if (!string.IsNullOrWhiteSpace(newVersion))
            {
                sb.AppendLine($"echo Updating appsettings.json version to {newVersion}...");
                sb.AppendLine($"powershell -Command \"if (Test-Path '{appSettingsPath}') {{ (Get-Content '{appSettingsPath}') -replace '(?<=\\\"CurrentVersion\\\":\\s*\\\")[^\\\"]*', '{newVersion}' | Set-Content '{appSettingsPath}' }}\"");
            }
            sb.AppendLine("timeout /t 1 /nobreak > nul");
            sb.AppendLine("echo Restarting MSFC.exe...");
            sb.AppendLine($"start \"\" \"{mainExeName}\"");
            sb.AppendLine("exit");

            File.WriteAllText(batchPath, sb.ToString(), Encoding.UTF8);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{batchPath}\"",
                CreateNoWindow = false,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process.Start(psi);
            Environment.Exit(0);
        }
    }
}
