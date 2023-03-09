using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UsingDotNET.DirMover.ViewModels;

namespace UsingDotNET.DirMover
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //Services = ConfigureServices();
        }

        //public new static App Current => (App)Application.Current;

        //public IServiceProvider Services { get; }

        //private static IServiceProvider ConfigureServices()
        //{
        //    //var services = new ServiceCollection();

        //    // DataContext
        //    //services.AddTransient<IToDoDataContext, ToDoDataContext>();

        //    // ViewModels
        //    //services.AddTransient<MainViewModel>();
        //    //services.AddTransient<NewToDoViewModel>();

        //    //return services.BuildServiceProvider();
        //}
    }
}