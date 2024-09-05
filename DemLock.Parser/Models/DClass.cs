using DemLock.Entities;
using DemLock.Utils;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a class in the context of a demo file, it is given
/// the D prefix simply to indicate this is vastly different from
/// the built in class types
/// </summary>
public class DClass
{
    public string? ClassName { get; set; }
    public int ClassId { get; set; }
}