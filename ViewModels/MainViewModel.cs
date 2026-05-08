using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows;
using CemuLauncher.Helpers;
using CemuLauncher.Models;
using CemuLauncher.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CemuLauncher.ViewModels;

public partial class MainViewModel : ObservableObject {
    public Cemu? Cemu { get; set; }

    [ObservableProperty]
    private string? status;

    [ObservableProperty]
    private bool progressIsIndeterminate = true;

    [ObservableProperty]
    private double progressValue = -1;

    public IProgress<double> Progress { get; }

    private Config? _config;

    private readonly ConfigLoader _configLoader;
    private readonly HttpClient _httpClient;
    private readonly Downloader _downloader;
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private readonly Task _initTask;

    public MainViewModel() {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CemuLauncher",
            Assembly.GetExecutingAssembly().GetName().Version?.ToString()));

        _downloader = new Downloader(_httpClient);

        _configLoader = new ConfigLoader(_deserializer);

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

        _initTask = InitializeAsync();
    }

    public async Task OnWindowLoaded() {
        await _initTask;

        var newVersion = await CemuManager.GetLatestCommitAsync(_httpClient)
            ?? throw new ArgumentNullException(Strings.UpdateCheck);

        if (Cemu!.DoUpdate(newVersion))
            await Cemu.InstallAsync(newVersion, Progress);

        Cemu.Launch();

        Application.Current.Shutdown();
    }

    private async Task InitializeAsync() {
        _config = await _configLoader.LoadConfigAsync();
        Cemu = await CemuManager.GetLocalCemuAsync(_config, _downloader);
    }
}
