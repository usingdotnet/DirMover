using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace UsingDotNET.DirMover.Models;

[Serializable]
public partial class LinkedDir:ObservableObject,ICloneable
{
    [ObservableProperty]
    private int _id;

    [ObservableProperty] 
    private string _name;

    [ObservableProperty] 
    private string _app;

    [ObservableProperty]
    private string _link;

    [ObservableProperty] 
    private string _target;

    [ObservableProperty]
    private LinkType _type;

    public DateTime TimeCreated { get; set; }

    public DateTime TimeUpdated { get; set; }

    private long _size;

    [JsonIgnore]
    public long Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public LinkedDir()
    {
        Id = 0;
        Name = "";
        App = "";
        Link = "";
        Target = "";
        Type = LinkType.D;
        TimeCreated = DateTime.Now;
        TimeUpdated = DateTime.Now;
    }

    public LinkedDir(string name, string app, string link, string target, LinkType type, DateTime timeCreated)
    {
        Id = 0;
        Name = name;
        App = app;
        Link = link;
        Target = target;
        Type = type;
        TimeCreated = timeCreated;
        TimeUpdated = DateTime.Now;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}