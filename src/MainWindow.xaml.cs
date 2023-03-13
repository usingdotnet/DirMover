using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using UsingDotNET.DirMover.ViewModels;

namespace UsingDotNET.DirMover;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
    }
}