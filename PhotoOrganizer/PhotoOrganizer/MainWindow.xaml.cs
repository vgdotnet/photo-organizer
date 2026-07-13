using Microsoft.UI.Xaml;
using System;
using System.IO;
using PhotoOrganizer.ViewModels;

namespace PhotoOrganizer {
    public sealed partial class MainWindow : Window {
        public MainViewModel ViewModel { get; }

        public MainWindow() {
            InitializeComponent();
            ViewModel = App.GetService<MainViewModel>();
            RootGrid.DataContext = ViewModel;
            Title = ViewModel.Title;
            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico"));
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1280, 860));
        }
    }
}
