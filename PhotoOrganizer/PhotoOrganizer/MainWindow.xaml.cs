using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using PhotoOrganizer.ViewModels;

namespace PhotoOrganizer {
    public sealed partial class MainWindow : Window {
        private readonly ILogger<MainWindow> _logger;

        public MainViewModel ViewModel { get; }

        public MainWindow() {
            InitializeComponent();
            _logger = App.GetService<ILogger<MainWindow>>();
            ViewModel = App.GetService<MainViewModel>();
            RootGrid.DataContext = ViewModel;
            Title = ViewModel.Title;
            AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico"));
            AppWindow.Resize(new Windows.Graphics.SizeInt32(1280, 860));
        }

        private async void Tree_Expanding(TreeView sender, TreeViewExpandingEventArgs args) {
            try {
                if (args.Item is FolderNode node) {
                    await ViewModel.LoadChildrenAsync(node);
                }
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot load the children of the expanded node");
            }
        }

        private void DestinationTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            if (args.InvokedItem is FolderNode node) {
                ViewModel.SelectedDestination = node;
            }
        }

        private async void SourcesTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            try {
                if (args.InvokedItem is FolderNode node) {
                    await ViewModel.ShowFolderAsync(node);
                }
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot show the folder invoked in the sources tree");
            }
        }

        private async void Tile_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) {
            try {
                if (sender is FrameworkElement element && element.DataContext is ContentTile tile) {
                    await ViewModel.OpenAsync(tile);
                    BringSelectedSourceIntoView();
                }
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot open the double-tapped tile");
            }
        }

        private async void Breadcrumbs_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args) {
            try {
                if (args.Item is PathSegment segment) {
                    await ViewModel.NavigateAsync(segment);
                    BringSelectedSourceIntoView();
                }
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot navigate to the clicked breadcrumb");
            }
        }

        private async void BringSelectedSourceIntoView() {
            try {
                for (var attempt = 0; attempt < 20; attempt++) {
                    if (TryBringSelectedSourceIntoView()) {
                        return;
                    }

                    await Task.Delay(50);
                }

                _logger.LogDebug("The selected node has no container yet, scrolling skipped");
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot scroll the sources tree to the selected node");
            }
        }

        private bool TryBringSelectedSourceIntoView() {
            var selected = ViewModel.SelectedSourceItem;

            if (selected is null) {
                return true;
            }

            var container = selected is TreeViewNode node
                ? SourcesTree.ContainerFromNode(node)
                : SourcesTree.ContainerFromItem(selected);

            if (container is not FrameworkElement element) {
                return false;
            }

            element.StartBringIntoView(new BringIntoViewOptions {
                VerticalAlignmentRatio = 0.5,
                AnimationDesired = true,
            });

            return true;
        }

        private void Tiles_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args) {
            if (args.InRecycleQueue) {
                return;
            }

            if (args.Phase == 0) {
                args.RegisterUpdateCallback(LoadTileThumbnail);
            }
        }

        private async void LoadTileThumbnail(ListViewBase sender, ContainerContentChangingEventArgs args) {
            try {
                if (args.Item is PhotoTile tile) {
                    await tile.LoadThumbnailAsync();
                }
            }
            catch (Exception exception) {
                _logger.LogError(exception, "Cannot load a tile thumbnail");
            }
        }
    }
}
