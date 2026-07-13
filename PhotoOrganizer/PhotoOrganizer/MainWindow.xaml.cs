using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        private async void DestinationTree_Expanding(TreeView sender, TreeViewExpandingEventArgs args) {
            if (args.Item is FolderNode node) {
                await ViewModel.LoadChildrenAsync(node);
            }
        }

        private void DestinationTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            if (args.InvokedItem is FolderNode node) {
                ViewModel.SelectedDestination = node;
            }
        }
    }
}
