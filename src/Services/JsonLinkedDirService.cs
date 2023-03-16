using System.IO;
using System.Text;
using Newtonsoft.Json;
using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover.Services;

public class JsonLinkedDirService : ILinkedDirService
{
    private List<LinkedDir> _linkedDirs = new List<LinkedDir>();
    private const string ConfigFile = "config.json";

    public IEnumerable<LinkedDir> GetAll()
    {
        _linkedDirs.Clear();
        if (File.Exists(ConfigFile))
        {
            string content = File.ReadAllText(ConfigFile, Encoding.UTF8);
            _linkedDirs.AddRange(JsonConvert.DeserializeObject<List<LinkedDir>>(content) ?? new List<LinkedDir>());
            _linkedDirs = _linkedDirs.OrderBy(x => x.TimeCreated).ToList();
        }
        else
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _linkedDirs.AddRange(Utility.TraverseTree(userProfile));
            Save();
        }

        return _linkedDirs;
    }

    public int Add(LinkedDir linkedDir)
    {
        int maxId = _linkedDirs.Select(l => l.Id).Max();
        linkedDir.Id = maxId + 1;
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
        LinkedDir x = _linkedDirs.SingleOrDefault(q => q.Id == ld.Id);
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
        LinkedDir x = _linkedDirs.SingleOrDefault(q => q.Id == ld.Id);
        if (x != null)
        {
            _linkedDirs.Remove(x);
            Save();
        }
    }

    public LinkedDir Get(int id)
    {
        LinkedDir x = _linkedDirs.SingleOrDefault(q => q.Id == id);
        if (x != null)
        {
            return x;
        }

        return new LinkedDir();
    }

    private void Save()
    {
        string str = JsonConvert.SerializeObject(_linkedDirs, Formatting.Indented);
        File.WriteAllText(ConfigFile, str);
    }
}