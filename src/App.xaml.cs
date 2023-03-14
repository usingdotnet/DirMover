using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UsingDotNET.DirMover.Services;
using UsingDotNET.DirMover.ViewModels;

namespace UsingDotNET.DirMover;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Services = ConfigureServices();
    }

    public new static App Current => (App)Application.Current;

    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Services
        services.AddTransient<ILinkedDirService, JsonLinkedDirService>();

        // ViewModels
        services.AddTransient<MainViewModel>();

        return services.BuildServiceProvider();
    }
}