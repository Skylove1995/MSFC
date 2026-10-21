using System.Text.Json.Serialization;

namespace MSFC.Dto
{
    public class UpdateCheckResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("hasUpdate")]
        public bool HasUpdate { get; set; }

        [JsonPropertyName("currentVersion")]
        public string CurrentVersion { get; set; } = string.Empty;

        [JsonPropertyName("latestVersion")]
        public string LatestVersion { get; set; } = string.Empty;

        [JsonPropertyName("downloadUrl")]
        public string DownloadUrl { get; set; } = string.Empty;

        [JsonPropertyName("sha256")]
        public string Sha256 { get; set; } = string.Empty;

        [JsonPropertyName("fileSize")]
        public long FileSize { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("releaseNotes")]
        public string ReleaseNotes { get; set; } = string.Empty;

        [JsonPropertyName("uploadedAt")]
        public string UploadedAt { get; set; } = string.Empty;
    }
}
