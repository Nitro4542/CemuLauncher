using CemuLauncher.Helpers;
using CemuLauncher.Resources;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace CemuLauncher.Models
{
    public sealed class Cemu(Config config, Downloader downloader)
    {
        public string? Version { get; set; }

        private const string DownloadUrl =
            "https://nightly.link/cemu-project/Cemu/workflows/build_check/main/cemu-bin-windows-x64.zip";

        private const string VersionFileName = "version.txt";
        private const string ZipFileName = "cemu-bin-windows-x64.zip";

        public string CemuPath = config.CemuPath;
        private string ExecutablePath =>
            Path.Combine(CemuPath, "Cemu.exe");
        private string DownloadPath =>
            Path.Combine(CemuPath, config.DownloadPath);
        private string ZipFilePath =>
            Path.Combine(DownloadPath, ZipFileName);
        private string VersionFilePath =>
            Path.Combine(CemuPath, VersionFileName);

        public void Launch(bool passArguments = true)
        {
            if (!File.Exists(ExecutablePath))
                throw new FileNotFoundException(Strings.Error_CemuNotFound, ExecutablePath);

            var startInfo = new ProcessStartInfo()
            {
                FileName = ExecutablePath,
                UseShellExecute = true
            };

            if (passArguments)
            {
                foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
                    startInfo.ArgumentList.Add(arg);
            }

            Process.Start(startInfo);
        }

        public async Task SetLocalVersionAsync()
        {
            if (!File.Exists(VersionFilePath))
                return;

            try
            {
                Version = await File.ReadAllTextAsync(VersionFilePath);
            }
            catch { }
        }

        public bool DoUpdate(string newVersion)
        {
            var update = newVersion != Version;

            if (update && config.UpdatePrompt)
                update = PromptUpdate();

            return update;
        }

        public async Task InstallAsync(string newVersion, IProgress<double>? downloadProgress = null)
        {
            Version = newVersion;

            Directory.CreateDirectory(CemuPath);
            Directory.CreateDirectory(DownloadPath);

            await downloader.DownloadAsync(
                DownloadUrl, DownloadPath, ZipFileName, downloadProgress);

            await UnpackAsync();

            ApplyOptions();

            await CleanupAsync();
        }

        private async Task UnpackAsync()
        {
            if (File.Exists(ExecutablePath))
                File.Delete(ExecutablePath);

            await ZipFile.ExtractToDirectoryAsync(ZipFilePath, CemuPath, overwriteFiles: true);
        }

        private void ApplyOptions()
        {
            if (config.Portable)
                Directory.CreateDirectory(Path.Combine(CemuPath, "portable"));
        }

        private static bool PromptUpdate() => MessageBox.Show(
            Strings.UpdatePrompt,
            Strings.UpdateAvailable,
            MessageBoxButton.YesNo,
            MessageBoxImage.Information)
            == MessageBoxResult.Yes;

        private async Task CleanupAsync()
        {
            if (File.Exists(ZipFilePath))
                File.Delete(ZipFilePath);

            if (Version != null)
                await File.WriteAllTextAsync(VersionFilePath, Version);
        }
    }
}
