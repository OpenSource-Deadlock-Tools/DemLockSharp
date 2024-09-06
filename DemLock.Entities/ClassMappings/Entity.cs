namespace DemLock.Entities.ClassMappings;

public class Entity
{
    public int Index { get; set; }
    public virtual void UpdateProperty(ReadOnlySpan<int> path, object value)
    { }
}