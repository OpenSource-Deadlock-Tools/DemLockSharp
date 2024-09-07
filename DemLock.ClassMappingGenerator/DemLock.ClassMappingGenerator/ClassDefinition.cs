using System.Collections.Generic;

namespace DemLock.ClassMappingGenerator;

internal class ClassDefinition
{
    public string ClassName { get; set; }
    public int Version { get; set; }
    public Dictionary<string, FieldDefinition> Fields { get; set; }
}