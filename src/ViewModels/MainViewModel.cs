using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CliWrap;
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
    private ObservableCollection<LinkedDir> _linkedDirs = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MoveCommand))]
    private LinkedDir? _selectedDir;

    [RelayCommand]
    private void Changed(LinkedDir dir)
    {
        var a = 33;
    }

    [RelayCommand(CanExecute = nameof(CanMove))]
    private void Move()
    {
        var a = _selectedDir.Link;
        var b = _selectedDir.Target;

        Move1();
    }

    private async void Move1()
    {
        if (string.IsNullOrEmpty(_selectedDir?.Link) || string.IsNullOrEmpty(_selectedDir.Target))
        {
            return;
        }

        var link = _selectedDir.Link.TrimEnd('\\');
        var target = _selectedDir.Target.TrimEnd('\\');

        if (Directory.Exists(link))
        {
            MessageBox.Show("源文件夹不存在");
            return;
        }

        if (Directory.Exists(target))
        {
            MessageBox.Show("目标文件夹已存在");
            return;
        }

        FileInfo fiLink = new FileInfo(link);
        if (fiLink.LinkTarget != null)
        {
            var ot = fiLink.LinkTarget.TrimEnd('\\');
            var nt = target;
            if (ot == target)
            {
                MessageBox.Show("无需操作");
                return;
            }
            else
            {
                var di1 = new DirectoryInfo(ot);
                var di2 = new DirectoryInfo(nt);
                CopyDirectory(di1, di2);
                var backup = ot + BakSuffix;
                Directory.Move(ot, backup);
                Directory.Delete(link); // 删除目前的链接
            }
        }
        else
        {
            var di1 = new DirectoryInfo(link);
            var di2 = new DirectoryInfo(target);
            CopyDirectory(di1, di2);
            var backup = link + BakSuffix;
            Directory.Move(link, backup);
        }

        var task = Cli.Wrap("cmd")
            .WithArguments($"""/C mklink /d "{link}" "{target}" """)
            .ExecuteAsync();
        await task;
    }

    static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
    {
        if (!destination.Exists)
        {
            destination.Create();
        }

        // Copy all files.
        FileInfo[] files = source.GetFiles();
        foreach (FileInfo file in files)
        {
            file.CopyTo(Path.Combine(destination.FullName,
                file.Name));
        }

        // Process subdirectories.
        DirectoryInfo[] dirs = source.GetDirectories();
        foreach (DirectoryInfo dir in dirs)
        {
            // Get destination directory.
            string destinationDir = Path.Combine(destination.FullName, dir.Name);

            // Call CopyDirectory() recursively.
            CopyDirectory(dir, new DirectoryInfo(destinationDir));
        }
    }

    private bool CanMove()
    {
        return _selectedDir is not null;
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
                subDirs = Directory.GetDirectories(currentDir);
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
                    LinkedDirs.Add(new LinkedDir {Name = d.Name,App = d.Name, Link =d.FullName, Target = d.LinkTarget});
                }
                else
                {
                    dirs.Push(dir);
                }
            }
        }
    }
}