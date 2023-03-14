using System.IO;
using System.Text;
using Newtonsoft.Json;
using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover.Services;

public class JsonLinkedDirService : ILinkedDirService
{
    private readonly List<LinkedDir> _linkedDirs = new List<LinkedDir>();
    private const string ConfigFile = "config.json";

    public IEnumerable<LinkedDir> GetAll()
    {
        _linkedDirs.Clear();
        if (File.Exists(ConfigFile))
        {
            var content = File.ReadAllText(ConfigFile, Encoding.UTF8);
            _linkedDirs.AddRange(JsonConvert.DeserializeObject<List<LinkedDir>>(content) ?? new List<LinkedDir>());
        }
        else
        {
            _linkedDirs.AddRange(Utility.TraverseTree(@"C:\Users\Administrator\"));
            Save();
        }

        return _linkedDirs;
    }

    public int Add(LinkedDir linkedDir)
    {
        _linkedDirs.Add(linkedDir);
        Save();
        return 1;
    }

    public bool Contains(LinkedDir ld)
    {
        if (_linkedDirs.Any(x => x.Link == ld.Link && x.Target == ld.Target))
        {
            return true;
        }

        return false;
    }

    public void Update(LinkedDir ld)
    {
        var x = _linkedDirs.SingleOrDefault(q => q.Id == ld.Id);
        if (x != null)
        {
            x.Link = ld.Link;
            x.Target = ld.Target;
            x.App = ld.App;
            x.Name = ld.Name;
            x.Type = ld.Type;
            x.TimeUpdated = DateTime.Now;

            Save();
        }
    }

    public void Delete(LinkedDir ld)
    {
        var x = _linkedDirs.SingleOrDefault(q => q.Id == ld.Id);
        if (x != null)
        {
            _linkedDirs.Remove(x);
            Save();
        }
    }

    public LinkedDir Get(int id)
    {
        var x = _linkedDirs.SingleOrDefault(q => q.Id == id);
        if (x != null)
        {
            return x;
        }

        return new LinkedDir();
    }

    private void Save()
    {
        var str = JsonConvert.SerializeObject(_linkedDirs, Formatting.Indented);
        File.WriteAllText(ConfigFile, str);
    }
}