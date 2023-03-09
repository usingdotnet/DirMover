using System.IO;
using System.Windows;
using CliWrap;

namespace UsingDotNET.DirMover
{
    public partial class MainWindow : Window
    {
        private const string BakSuffix = ".dmbak";

        public MainWindow()
        {
            InitializeComponent();
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
    }
}