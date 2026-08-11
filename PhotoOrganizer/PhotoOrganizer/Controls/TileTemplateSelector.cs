using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PhotoOrganizer.ViewModels;

namespace PhotoOrganizer.Controls {
    public sealed partial class TileTemplateSelector : DataTemplateSelector {
        public DataTemplate? FolderTemplate { get; set; }

        public DataTemplate? PhotoTemplate { get; set; }

        protected override DataTemplate? SelectTemplateCore(object item) {
            return item is FolderTile ? FolderTemplate : PhotoTemplate;
        }

        protected override DataTemplate? SelectTemplateCore(object item, DependencyObject container) {
            return SelectTemplateCore(item);
        }
    }
}
