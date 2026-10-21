using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MSFC.Data;
using MSFC.Service;
using MSFC.Service.Translate;

namespace MSFC
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += (s, e) =>
            {
                ShowFriendlyError(e.Exception, "System Error / Error del Sistema");
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception ?? new Exception(e.ExceptionObject?.ToString());
                ShowFriendlyError(ex, "Unhandled Error / Error No Controlado");
            };


            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();

                var host = Host.CreateDefaultBuilder()
                    // đảm bảo content root đúng khi chạy từ nơi khác
                    .UseContentRoot(AppContext.BaseDirectory)
                    .ConfigureServices((ctx, services) =>
                    {

                        services.AddSingleton<IConfiguration>(configuration);
                        services.Configure<AutomationConfig2>(configuration.GetSection("AutomationConfig2")); // ui config for SFC
                        services.Configure<KlasUiConfig>(configuration.GetSection("KLAS_UiConfig")); // ui config for KLAS
                        services.Configure<TranslationConfig>(configuration.GetSection("TranslationConfig"));


                        var conn = configuration.GetConnectionString("DefaultConnection");
                        var serverVersion = GetServerVersionWithRetry(conn);
                        services.AddDbContextFactory<MMesDbContext>(o => o.UseMySql(
                            conn, 
                            serverVersion,
                            builder => builder.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(3),
                                errorNumbersToAdd: null
                            )
                        ));

                        // Trì hoãn UI automation: đừng làm việc nặng trong ctor/singleton
                        services.AddSingleton<IAutomationService2, AutomationService2>();
                        services.AddSingleton<ITranslatorService, TranslatorService>();

                        services.AddTransient<Form1>();
                    })
                    .Build();

                ApplicationConfiguration.Initialize();
                var form = host.Services.GetRequiredService<Form1>();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                ShowFriendlyError(ex, "Startup Error / Error de Inicio");
            }
        }

        private static ServerVersion GetServerVersionWithRetry(string? connectionString, int maxRetries = 5, int delayMs = 1000)
        {
            if (string.IsNullOrEmpty(connectionString))
                return ServerVersion.Parse("8.0.35-mysql");

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return ServerVersion.AutoDetect(connectionString);
                }
                catch
                {
                    if (attempt == maxRetries)
                    {
                        // Khi vượt quá số lần retry vẫn không AutoDetect được (ví dụ do mạng phản hồi chậm lúc khởi động),
                        // Fallback về phiên bản MySQL 8.0 tiêu chuẩn để ứng dụng khởi động thành công thay vì crash ngay.
                        return ServerVersion.Parse("8.0.35-mysql");
                    }
                    System.Threading.Thread.Sleep(delayMs);
                }
            }

            return ServerVersion.Parse("8.0.35-mysql");
        }

        private static void ShowFriendlyError(Exception ex, string title)
        {
            string exStr = ex.ToString();
            string userMessageEng;
            string userMessageEsp;

            if (exStr.Contains("MySql") || exStr.Contains("Timeout") || exStr.Contains("SocketException") || exStr.Contains("DbContext"))
            {
                userMessageEng = "[DATABASE CONNECTION ERROR]\n" +
                                 "Unable to connect to the database server. Please check your network/VPN connection and verify database settings in 'appsettings.json'.";

                userMessageEsp = "[ERROR DE CONEXIÓN A LA BASE DE DATOS]\n" +
                                 "No se pudo conectar al servidor de base de datos. Por favor verifique su conexión de red/VPN y la configuración en 'appsettings.json'.";
            }
            else if (exStr.Contains("appsettings") || exStr.Contains("FileNotFound"))
            {
                userMessageEng = "[CONFIGURATION FILE ERROR]\n" +
                                 "Missing or invalid configuration file. Please check 'appsettings.json'.";

                userMessageEsp = "[ERROR DE ARCHIVO DE CONFIGURACIÓN]\n" +
                                 "Archivo de configuración no encontrado o inválido. Por favor verifique 'appsettings.json'.";
            }
            else
            {
                userMessageEng = "[APPLICATION STARTUP ERROR]\n" +
                                 "An error occurred while initializing the application.";

                userMessageEsp = "[ERROR DE INICIO DE LA APLICACIÓN]\n" +
                                 "Ocurrió un error al inicializar la aplicación.";
            }

            string fullMessage = $"{userMessageEng}\n\n" +
                                $"{userMessageEsp}\n\n" +
                                $"--------------------------------------------------\n" +
                                $"[Technical Details / Detalles Técnicos]:\n" +
                                $"{ex.GetType().Name}: {ex.Message}";

            MessageBox.Show(fullMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}