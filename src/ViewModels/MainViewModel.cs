using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using CliWrap;
using UsingDotNET.DirMover.Models;
using UsingDotNET.DirMover.Services;

namespace UsingDotNET.DirMover.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private const string BakSuffix = ".dmbak";
    private readonly ILinkedDirService _linkedDirService;

    public MainViewModel(ILinkedDirService linkedDirService)
    {
        _linkedDirService = linkedDirService;
        RegisterMessages();
        LoadLinkedDirs();
    }

    [ObservableProperty]
    private ObservableCollection<LinkedDir> _linkedDirs = new();

    [ObservableProperty]
    private LinkedDir _selectedDir;

    [ObservableProperty]
    private LinkedDir _currentLinkedDir = new LinkedDir();

    [RelayCommand]
    private void Changed(LinkedDir dir)
    {
        if (dir != null)
        {
            CurrentLinkedDir = (LinkedDir)dir.Clone();
            CurrentLinkedDir.Id = dir.Id;
        }
        else
        {
            CurrentLinkedDir = new LinkedDir();
        }
    }

    [RelayCommand]
    private void NewLinkedDir()
    {
        CurrentLinkedDir = new LinkedDir();
    }

    [RelayCommand]
    private async void Move()
    {
        LinkedDir old = null;
        if (CurrentLinkedDir!.Id != 0)
        {
            old = _linkedDirService.Get(CurrentLinkedDir.Id);
        }

        if (string.IsNullOrEmpty(CurrentLinkedDir?.Link) || string.IsNullOrEmpty(CurrentLinkedDir.Target))
        {
            return;
        }

        var link = CurrentLinkedDir.Link.TrimEnd('\\');
        var target = CurrentLinkedDir.Target.TrimEnd('\\');

        if (!Directory.Exists(link))
        {
            MessageBox.Show("The source dir does not exist");
            return;
        }

        if (Directory.Exists(target))
        {
            //MessageBox.Show("目标文件夹已存在");
            //return;
        }

        var onlyInfo = false;
        FileInfo fiLink = new FileInfo(link);

        if (old != null && old.Target == target && fiLink.FullName == old.Link && CurrentLinkedDir.Type == old.Type)
        {
            onlyInfo = true;
        }

        if (!onlyInfo)
        {
            if (fiLink.LinkTarget != null)
            {
                var ot = fiLink.LinkTarget.TrimEnd('\\');
                var nt = target;

                if (ot != nt)
                {
                    CopyDirectory(ot, nt);
                    Directory.Delete(ot);
                }

                if (old != null && CurrentLinkedDir.Type != old.Type)
                {
                    Directory.Delete(link, true); // 删除目前的链接
                    await CreateLink(link, target);
                }
            }
            else
            {
                CopyDirectory(link, target);
                Directory.Delete(link, true);
                await CreateLink(link, target);
            }
        }

        if (!_linkedDirService.Contains(CurrentLinkedDir))
        {
            _linkedDirService.Add(CurrentLinkedDir);
        }
        else
        {
            _linkedDirService.Update(CurrentLinkedDir);
        }

        LoadLinkedDirs();
    }

    [RelayCommand]
    private void Cancel()
    {
        var r =  MessageBox.Show("Do you really want to cancel current link and move dir to it's original place?","waring",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.No);
        if (r == MessageBoxResult.Yes)
        {
            var ld = _linkedDirService.Get(_selectedDir.Id);
            var link = CurrentLinkedDir.Link.TrimEnd('\\');
            Directory.Delete(link, true); // 删除链接

            var target = CurrentLinkedDir.Target.TrimEnd('\\');

            CopyDirectory(target, link);
            Directory.Delete(target,true);
            _linkedDirService.Delete(ld);
            CurrentLinkedDir = new LinkedDir();
            LoadLinkedDirs();
        }
    }

    private static void CopyDirectory(string link, string target)
    {
        var di1 = new DirectoryInfo(link);
        var di2 = new DirectoryInfo(target);
        CopyDirectory(di1, di2);
    }

    private async Task CreateLink(string link, string target)
    {
        var method = $@"/{CurrentLinkedDir.Type.ToString()}";
        var task = Cli.Wrap("cmd")
            .WithArguments($"""/C mklink {method} "{link}" "{target}" """)
            .ExecuteAsync();
        await task;
    }

    static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
    {
        if (!destination.Exists)
        {
            destination.Create();
        }

        FileInfo[] files = source.GetFiles();
        foreach (FileInfo file in files)
        {
            file.CopyTo(Path.Combine(destination.FullName,
                file.Name));
        }

        DirectoryInfo[] dirs = source.GetDirectories();
        foreach (DirectoryInfo dir in dirs)
        {
            string destinationDir = Path.Combine(destination.FullName, dir.Name);
            CopyDirectory(dir, new DirectoryInfo(destinationDir));
        }
    }

    /// <summary>
    /// Loads existing ToDos from the DataContext
    /// </summary>
    private void LoadLinkedDirs()
    {
        LinkedDirs.Clear();

        var ds = _linkedDirService.GetAll();
        foreach (var d in ds)
        {
            _linkedDirs.Add(d);
        }
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
}