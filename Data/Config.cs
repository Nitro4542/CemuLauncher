namespace cemu_launcher.Data
{
    public class Config
    {
        public bool ask_before_update { get; set; } = false;
        public string cemu_path { get; set; } = "cemu";
        public bool cemu_portable { get; set; } = true;
        public string download_path { get; set; } = "downloads";
    }
}
