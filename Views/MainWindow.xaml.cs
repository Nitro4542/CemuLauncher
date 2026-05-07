using cemu_launcher.ViewModels;
using System.Windows;

namespace cemu_launcher.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void OnWindowLoaded(object sender, RoutedEventArgs e) => await ViewModel.OnWindowLoaded();
    }
}