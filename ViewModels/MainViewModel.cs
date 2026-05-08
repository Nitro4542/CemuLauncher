using System.Net.Http;
using System.Windows;
using CemuLauncher.Helpers;
using CemuLauncher.Models;
using CemuLauncher.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CemuLauncher.ViewModels;

public partial class MainViewModel : ObservableObject {
    [ObservableProperty]
    private string? status;

    [ObservableProperty]
    private bool progressIsIndeterminate = true;

    [ObservableProperty]
    private double progressValue = -1;

    public IProgress<double> Progress { get; }

    private readonly Cemu _cemu;
    private readonly HttpClient _httpClient;

    public MainViewModel(Cemu cemu, IHttpClientFactory httpClientFactory) {
        _cemu = cemu;
        _httpClient = httpClientFactory.CreateClient("Default");

        Status = Strings.UpdateCheck;

        Progress = new Progress<double>(p => {
            if (p < 0) {
                ProgressIsIndeterminate = true;
            } else {
                Status = Strings.UpdateAvailable;
                ProgressIsIndeterminate = false;
                ProgressValue = p;
            }
        });
    }

    public async Task OnWindowLoadedAsync() {
        try {
            await _cemu.SetLocalVersionAsync();

            var newVersion = await CemuManager.GetLatestCommitAsync(_httpClient);

            if (_cemu.CheckUpdate(newVersion))
                await _cemu.InstallAsync(newVersion, Progress);

            _cemu.Launch();

            Application.Current.Shutdown();
        } catch (Exception ex) {
            Status = string.Join(" ", [Strings.ErrorPrefix, ex.Message]);
        }
    }
}
