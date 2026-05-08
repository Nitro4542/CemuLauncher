using System.Windows;
using CemuLauncher.ViewModels;

namespace CemuLauncher.Views;

public partial class MainWindow : Window {
    public MainWindow(MainViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;

        _ = viewModel.OnWindowLoadedAsync();
    }
}
