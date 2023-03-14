using UsingDotNET.DirMover.Models;

namespace UsingDotNET.DirMover.Services;

public interface ILinkedDirService
{
    IEnumerable<LinkedDir> GetAll();

    int Add(LinkedDir linkedDir);

    bool Contains(LinkedDir ld);

    void Update(LinkedDir ld);

    void Delete(LinkedDir ld);

    LinkedDir Get(int id);
}