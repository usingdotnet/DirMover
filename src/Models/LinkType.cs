using System.ComponentModel;

namespace UsingDotNET.DirMover.Models;

public enum LinkType
{
    [Description("/D")]
    D,

    [Description("/J")]
    J,

    [Description("/H")]
    H,
}