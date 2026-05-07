using cemu_launcher.Data;
using cemu_launcher.Helpers;
using cemu_launcher.Models;
using cemu_launcher.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Windows;

namespace cemu_launcher.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public Cemu? Cemu { get; set; }

        [ObservableProperty]
        private string? status;

        [ObservableProperty]
        private bool progressIsIndeterminate = true;

        [ObservableProperty]
        private double progressValue = -1;

        public IProgress<double> Progress { get; }

        private readonly Config _config = ConfigLoader.LoadConfig();
        private readonly HttpClient _httpClient;
        private readonly Downloader _downloader;

        private readonly Task _initTask;

        public MainViewModel()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("cemu-launcher",
                Assembly.GetExecutingAssembly().GetName().Version?.ToString()));

            _downloader = new Downloader(_httpClient);

            Status = Strings.UpdateCheck;

            Progress = new Progress<double>(p =>
            {
                if (p < 0)
                {
                    ProgressIsIndeterminate = true;
                }
                else
                {
                    Status = Strings.UpdateAvailable;
                    ProgressIsIndeterminate = false;
                    ProgressValue = p;
                }
            });

            _initTask = InitializeAsync();
        }

        public async Task OnWindowLoaded()
        {
            await _initTask;

            var newVersion = await CemuManager.GetLatestCommitAsync(_httpClient)
                ?? throw new ArgumentNullException(Strings.UpdateCheck);

            if (Cemu!.DoUpdate(newVersion))
                await Cemu.InstallAsync(newVersion, Progress);

            Cemu.Launch();

            Application.Current.Shutdown();
        }

        private async Task InitializeAsync()
        {
            Cemu = await CemuManager.GetLocalCemuAsync(_config, _downloader);
        }
    }
}
