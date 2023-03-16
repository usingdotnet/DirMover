using System.IO;
using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover;

internal static class Utility
{
    private  static readonly List<string> SpecialFolders;
    static Utility()
    {
        SpecialFolders = GetAllSpecialFolders();
    }

    public static List<LinkedDir> TraverseTree(string root)
    {
        int id = 1;
        var r = new List<LinkedDir>();
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
                    var ld = new LinkedDir(d.Name, d.Name, d.FullName, d.LinkTarget, LinkType.D, d.CreationTime);
                    ld.Id = id++;
                    r.Add(ld);
                }
                else
                {
                    dirs.Push(dir);
                }
            }
        }

        r = r.OrderBy(x => x.TimeUpdated).ToList();
        return r;
    }

    public static bool IsSymbolicLink(string path)
    {
        FileInfo file = new FileInfo(path);
        if (file.LinkTarget != null)
        {
            return true;
        }

        return false;
    }

    public static List<string> GetAllSpecialFolders()
    {
        var folders = Enum.GetValues(typeof(Environment.SpecialFolder))
            .Cast<Environment.SpecialFolder>()
            .Select(specialFolder => new
            {
                Name = specialFolder.ToString(),
                Path = Environment.GetFolderPath(specialFolder)
            })
            .OrderBy(item => item.Path.ToLower()).Where(x => !string.IsNullOrEmpty(x.Path)).Select(x=> x.Path);

        return folders.ToList();
    }

    public static bool IsParentfolder(this string parentPath, string childPath)
    {
        var parentUri = new Uri(parentPath);
        var childUri = new DirectoryInfo(childPath).Parent;
        while (childUri != null)
        {
            if (new Uri(childUri.FullName) == parentUri)
            {
                return true;
            }
            childUri = childUri.Parent;
        }
        return false;
    }

    public static bool IsSpecialFolder(this string path)
    {
        return SpecialFolders.Contains(path, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsParentOfSpecialFolder(this string path)
    {
        return SpecialFolders.Any(sp => path.IsParentfolder(sp));
    }

    public static long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }

        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }

        return size;
    }

    public static string GetBytesReadable(this long i, int digits = 2)
    {
        // Get absolute value
        long abs = (i < 0 ? -i : i);
        // Determine the suffix and readable value
        string suffix;
        double readable;
        if (abs >= 0x1000000000000000) // Exabyte
        {
            suffix = "EB";
            readable = (i >> 50);
        }
        else if (abs >= 0x4000000000000) // Petabyte
        {
            suffix = "PB";
            readable = (i >> 40);
        }
        else if (abs >= 0x10000000000) // Terabyte
        {
            suffix = "TB";
            readable = (i >> 30);
        }
        else if (abs >= 0x40000000) // Gigabyte
        {
            suffix = "GB";
            readable = (i >> 20);
        }
        else if (abs >= 0x100000) // Megabyte
        {
            suffix = "MB";
            readable = (i >> 10);
        }
        else if (abs >= 0x400) // Kilobyte
        {
            suffix = "KB";
            readable = i;
        }
        else
        {
            return i.ToString("0 B"); // Byte
        }

        // Divide by 1024 to get fractional value
        readable /= 1024;

        // Return formatted number with suffix
        var formatStr = "0.";
        for (int j = 0; j < digits; j++)
        {
            formatStr += '#';
        }

        return readable.ToString(formatStr) + " " + suffix;
    }

}