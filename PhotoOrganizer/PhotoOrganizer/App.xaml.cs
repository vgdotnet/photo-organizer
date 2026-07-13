using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using PhotoOrganizer.ViewModels;
using System;

namespace PhotoOrganizer {
    public partial class App : Application {
        private Window? _window;

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public App() {
            InitializeComponent();
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices() {
            var services = new ServiceCollection();

            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class {
            return Current.Services.GetService(typeof(T)) as T
                ?? throw new InvalidOperationException(
                    $"Service of type {typeof(T)} is not registered in the DI container.");
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
