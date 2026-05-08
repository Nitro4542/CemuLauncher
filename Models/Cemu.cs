using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using CemuLauncher.Resources;
using CemuLauncher.Services;

namespace CemuLauncher.Models;

public sealed class Cemu {
    public string? Version { get; set; }

    private const string DownloadUrl =
        "https://nightly.link/cemu-project/Cemu/workflows/build_check/main/cemu-bin-windows-x64.zip";

    private const string VersionFileName = "version.txt";
    private const string ZipFileName = "cemu-bin-windows-x64.zip";

    private string BasePath { get; } = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData), "CemuLauncher");
    private string CemuPath =>
        Path.IsPathRooted(_config!.CemuPath)
            ? _config!.CemuPath
            : Path.Combine(BasePath, _config!.CemuPath);
    private string ExecutablePath =>
        Path.Combine(CemuPath, "Cemu.exe");
    private string DownloadPath =>
        Path.IsPathRooted(_config!.DownloadPath)
            ? _config!.DownloadPath
            : Path.Combine(BasePath, _config!.DownloadPath);
    private string ZipFilePath =>
        Path.Combine(DownloadPath, ZipFileName);
    private string VersionFilePath =>
        Path.Combine(BasePath, VersionFileName);

    private readonly Downloader _downloader;
    private Config? _config;

    public Cemu(ConfigService configService, Downloader downloader) {
        _downloader = downloader;

        _ = InitializeAsync(configService);
    }

    private async Task InitializeAsync(ConfigService configService) {
        _config = await configService.GetAsync();
    }

    public void Launch(bool passArguments = true) {
        if (!File.Exists(ExecutablePath))
            throw new FileNotFoundException(Strings.Error_CemuNotFound, ExecutablePath);

        var startInfo = new ProcessStartInfo() {
            FileName = ExecutablePath,
            UseShellExecute = true
        };

        if (passArguments) {
            foreach (var arg in Environment.GetCommandLineArgs().Skip(1))
                startInfo.ArgumentList.Add(arg);
        }

        Process.Start(startInfo);
    }

    public async Task SetLocalVersionAsync() {
        if (!File.Exists(VersionFilePath))
            return;

        try {
            Version = await File.ReadAllTextAsync(VersionFilePath);
        } catch { }
    }

    public bool CheckUpdate(string? newVersion) {
        var update = Version == null || Version != newVersion;

        if (update && _config!.UpdatePrompt)
            update = PromptUpdate();

        return update;
    }

    public async Task InstallAsync(string? newVersion, IProgress<double>? downloadProgress = null) {
        Version = newVersion;

        Directory.CreateDirectory(BasePath);
        Directory.CreateDirectory(CemuPath);
        Directory.CreateDirectory(DownloadPath);

        await _downloader.DownloadAsync(
            DownloadUrl, DownloadPath, ZipFileName, downloadProgress);

        await UnpackAsync();

        ApplyOptions();

        await CleanupAsync();
    }

    private async Task UnpackAsync() {
        if (File.Exists(ExecutablePath))
            File.Delete(ExecutablePath);

        await ZipFile.ExtractToDirectoryAsync(ZipFilePath, CemuPath, overwriteFiles: true);
    }

    private void ApplyOptions() {
        var portablePath = Path.Combine(CemuPath, "portable");
        var disabledPath = Path.Combine(CemuPath, "portable.disabled");

        if (_config!.Portable)
            if (Path.Exists(disabledPath))
                Directory.Move(disabledPath, portablePath);
            else
                Directory.CreateDirectory(portablePath);
        else if (Path.Exists(portablePath))
            Directory.Move(portablePath, disabledPath);
    }

    private static bool PromptUpdate() =>
        MessageBox.Show(
            Strings.UpdatePrompt,
            Strings.UpdateAvailable,
            MessageBoxButton.YesNo,
            MessageBoxImage.Information)
        == MessageBoxResult.Yes;

    private async Task CleanupAsync() {
        if (File.Exists(ZipFilePath))
            File.Delete(ZipFilePath);

        if (Version != null)
            await File.WriteAllTextAsync(VersionFilePath, Version);
    }
}
