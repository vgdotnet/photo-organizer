using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotoOrganizer.Logging;
using PhotoOrganizer.Services;
using PhotoOrganizer.ViewModels;
using System;
using System.Threading.Tasks;

namespace PhotoOrganizer {
    public partial class App : Application {
        private readonly ILogger<App> _logger;

        private Window? _window;

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        public App() {
            InitializeComponent();
            Services = ConfigureServices();
            _logger = GetService<ILogger<App>>();

            RegisterExceptionHandlers();

            _logger.LogInformation("Photo Organizer started, log file: {Path}", GetService<LogFile>().FilePath);
        }

        private static IServiceProvider ConfigureServices() {
            var services = new ServiceCollection();

            var logFile = new LogFile();

            services.AddSingleton(logFile);

            services.AddLogging(builder => {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddProvider(new FileLoggerProvider(logFile));
            });

            services.AddSingleton<IFileSystemService, FileSystemService>();

            services.AddSingleton<IShellService, ShellService>();

            services.AddTransient<MainViewModel>();

            return services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : class {
            return Current.Services.GetService(typeof(T)) as T
                ?? throw new InvalidOperationException(
                    $"Service of type {typeof(T)} is not registered in the DI container.");
        }

        private void RegisterExceptionHandlers() {
            UnhandledException += (_, args) => {
                _logger.LogCritical(args.Exception, "Unhandled exception on the UI thread");
            };

            AppDomain.CurrentDomain.UnhandledException += (_, args) => {
                _logger.LogCritical(args.ExceptionObject as Exception, "Unhandled exception in the application domain");
            };

            TaskScheduler.UnobservedTaskException += (_, args) => {
                _logger.LogError(args.Exception, "Unobserved task exception");
                args.SetObserved();
            };
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
