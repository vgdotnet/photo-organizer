using System;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using PhotoOrganizer.Models;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace PhotoOrganizer.ViewModels;

public sealed partial class PhotoTile : ContentTile {
    private const uint ThumbnailSize = 256;

    private static readonly string[] SizeUnits = { "B", "KB", "MB", "GB", "TB" };

    private bool _thumbnailRequested;

    public PhotoTile(PhotoFile file) : base(file.Name, file.Path) {
        Meta = $"{FormatSize(file.Size)} · {file.Modified.LocalDateTime.ToString("MMM d, yyyy", CultureInfo.InvariantCulture)}";
    }

    public string Meta { get; }

    [ObservableProperty]
    public partial BitmapImage? Thumbnail { get; set; }

    public async Task LoadThumbnailAsync() {
        if (_thumbnailRequested) {
            return;
        }

        _thumbnailRequested = true;

        try {
            var file = await StorageFile.GetFileFromPathAsync(Path);
            using var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem, ThumbnailSize, ThumbnailOptions.UseCurrentScale);

            if (thumbnail is null || thumbnail.Type != ThumbnailType.Image) {
                return;
            }

            var image = new BitmapImage();
            await image.SetSourceAsync(thumbnail);
            Thumbnail = image;
        }
        catch (Exception) {
        }
    }

    private static string FormatSize(long bytes) {
        double size = bytes;
        var unit = 0;

        while (size >= 1024 && unit < SizeUnits.Length - 1) {
            size /= 1024;
            unit++;
        }

        var format = unit > 0 && size < 10 ? "0.0" : "0";

        return $"{size.ToString(format, CultureInfo.InvariantCulture)} {SizeUnits[unit]}";
    }
}
