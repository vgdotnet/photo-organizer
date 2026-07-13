using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace PhotoOrganizer.Controls {
    public sealed partial class TreeIcon : UserControl {
        public TreeIcon() {
            InitializeComponent();
        }

        public bool IsDrive {
            get => (bool)GetValue(IsDriveProperty);
            set => SetValue(IsDriveProperty, value);
        }

        public static readonly DependencyProperty IsDriveProperty =
            DependencyProperty.Register(nameof(IsDrive), typeof(bool), typeof(TreeIcon),
                new PropertyMetadata(false, OnIsDriveChanged));

        private static void OnIsDriveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var icon = (TreeIcon)d;
            var drive = (bool)e.NewValue;
            icon.Body.Fill = (Brush)icon.Resources[drive ? "DriveBody" : "FolderBody"];
            icon.Tab.Visibility = drive ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
