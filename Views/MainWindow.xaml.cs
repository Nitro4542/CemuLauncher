using System.Windows;
using CemuLauncher.ViewModels;

namespace CemuLauncher.Views;

public partial class MainWindow : Window {
    private MainViewModel ViewModel { get; } = new();

    public MainWindow() {
        InitializeComponent();
        DataContext = ViewModel;
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e) => await ViewModel.OnWindowLoaded();
}
