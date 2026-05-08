namespace CemuLauncher.Models
{
    public class Config
    {
        public bool UpdatePrompt { get; set; } = false;
        public string CemuPath { get; set; } = "cemu";
        public bool Portable { get; set; } = true;
        public string DownloadPath { get; set; } = "downloads";
    }
}
