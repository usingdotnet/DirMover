using System.IO;
using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover;

internal static class Utility
{
    private  static List<string> _specialFolders;
    static Utility()
    {
        _specialFolders = GetAllSpecialFolders();
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

    public static bool IsBaseOf(this string path1, string path2 )
    {
        Uri potentialBase = new Uri(path1);
        Uri regular = new Uri(path2);
        return potentialBase.IsBaseOf(regular);
    }

    public static bool IsSpecialFolder(this string path)
    {
        return _specialFolders.Contains(path, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsBaseOfSpecialFolder(this string path)
    {
        return _specialFolders.Any(sp => path.IsBaseOf(sp));
    }
}