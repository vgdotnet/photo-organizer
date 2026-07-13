using CommunityToolkit.Mvvm.ComponentModel;

namespace PhotoOrganizer.ViewModels;

public partial class MainViewModel : ObservableObject {
    public MainViewModel() {
        Title = "Photo Organizer";
    }

    [ObservableProperty]
    public partial string Title { get; set; }
}
