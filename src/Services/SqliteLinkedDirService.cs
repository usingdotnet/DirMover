using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover.Services;

public class SqliteLinkedDirService : ILinkedDirService
{
    private readonly List<LinkedDir> _linkedDirs = new List<LinkedDir>();

    public IEnumerable<LinkedDir> GetAll()
    {
        return _linkedDirs;
    }

    public int Add(LinkedDir linkedDir)
    {
        _linkedDirs.Add(linkedDir);
        return 1;
    }

    public bool Contains(LinkedDir ld)
    {
        throw new NotImplementedException();
    }

    public void Update(LinkedDir ld)
    {
        throw new NotImplementedException();
    }

    public void Delete(LinkedDir ld)
    {
        throw new NotImplementedException();
    }

    public LinkedDir Get(int id)
    {
        throw new NotImplementedException();
    }
}