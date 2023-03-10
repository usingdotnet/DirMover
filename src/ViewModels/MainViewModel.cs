using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string BakSuffix = ".dmbak";

    public MainViewModel()
    {
        RegisterMessages();
        LoadLinkedDirss();
    }

    [ObservableProperty]
    public ObservableCollection<LinkedDir> _linkedDirs = new();

    [RelayCommand]
    private void Changed()
    {
        var a = 33;
    }

    /// <summary>
    /// Loads existing ToDos from the DataContext
    /// </summary>
    private void LoadLinkedDirss()
    {
        LinkedDirs.Clear();
        var dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        TraverseTree(dir);
    }

    /// <summary>
    /// Register message to handle a new ToDo created message and adds to ToDos collection
    /// </summary>
    private void RegisterMessages()
    {
        //WeakReferenceMessenger.Default.Register<ToDoCreatedMessage>(this, (o, e) =>
        //{
        //    ToDos.Add(e.Value);
        //});
    }

    public void TraverseTree(string root)
    {
        Stack<string> dirs = new Stack<string>(20);

        if (!Directory.Exists(root))
        {
            throw new ArgumentException();
        }

        dirs.Push(root);

        while (dirs.Count > 0)
        {
            string currentDir = dirs.Pop();
            string[] subDirs;
            try
            {
                subDirs = System.IO.Directory.GetDirectories(currentDir);
            }
            catch (UnauthorizedAccessException e)
            {
                continue;
            }
            catch (DirectoryNotFoundException e)
            {
                continue;
            }

            foreach (string dir in subDirs)
            {
                var d = new DirectoryInfo(dir);
                if (d.LinkTarget != null && d.LinkTarget.First() != d.FullName.First() && !d.LinkTarget.StartsWith("Global"))
                {
                    Console.WriteLine(d.FullName.PadRight(70) + " => " + d.LinkTarget);
                    LinkedDirs.Add(new LinkedDir(){Name = d.Name});
                }
                else
                {
                    dirs.Push(dir);
                }
            }
        }
    }

}