namespace UsingDotNET.DirMover.Models;

public class LinkedDir
{
    public string Name { get; set; }

    public string App { get; set; }

    public string Link { get; set; }

    public string Target { get; set; }

    public LinkType Type { get; set; }

    public DateTime TimeCreated { get; set; }

    public LinkedDir(string name, string app, string link, string target, LinkType type, DateTime timeCreated)
    {
        Name = name;
        App = app;
        Link = link;
        Target = target;
        Type = type;
        TimeCreated = timeCreated;
    }
}