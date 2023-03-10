using System.IO;
using System.Windows;
using CliWrap;
using Microsoft.Extensions.DependencyInjection;
using UsingDotNET.DirMover.ViewModels;

namespace UsingDotNET.DirMover;

public partial class MainWindow : Window
{
    private const string BakSuffix = ".dmbak";
    //public ObservableCollection<string> _links = new();

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<MainViewModel>();
    }

    private async void BtnMove_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(TxtLink.Text) || string.IsNullOrEmpty(TxtTarget.Text))
        {
            return;
        }

        var link = TxtLink.Text.TrimEnd('\\');
        var target = TxtTarget.Text.TrimEnd('\\');

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
                CopyDirectory(di1,di2);
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

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        //var dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        // TraverseTree(dir);
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
                    //_links.Add(d.Name);
                }
                else
                {
                    dirs.Push(dir);
                }
            }
        }
    }
}