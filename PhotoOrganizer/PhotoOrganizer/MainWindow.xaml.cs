using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.IO;
using System.Threading.Tasks;
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

        private async void Tree_Expanding(TreeView sender, TreeViewExpandingEventArgs args) {
            if (args.Item is FolderNode node) {
                await ViewModel.LoadChildrenAsync(node);
            }
        }

        private void DestinationTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            if (args.InvokedItem is FolderNode node) {
                ViewModel.SelectedDestination = node;
            }
        }

        private async void SourcesTree_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args) {
            if (args.InvokedItem is FolderNode node) {
                await ViewModel.ShowFolderAsync(node);
            }
        }

        private async void Tile_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e) {
            if (sender is FrameworkElement element && element.DataContext is ContentTile tile) {
                await ViewModel.OpenAsync(tile);
                BringSelectedSourceIntoView();
            }
        }

        private async void Breadcrumbs_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args) {
            if (args.Item is PathSegment segment) {
                await ViewModel.NavigateAsync(segment);
                BringSelectedSourceIntoView();
            }
        }

        private async void BringSelectedSourceIntoView() {
            for (var attempt = 0; attempt < 20; attempt++) {
                if (TryBringSelectedSourceIntoView()) {
                    return;
                }

                await Task.Delay(50);
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
            if (args.Item is PhotoTile tile) {
                await tile.LoadThumbnailAsync();
            }
        }
    }
}
