namespace DemLock.Entities.ClassMappings;

public abstract class ClassMapping<T>
{
    public abstract void MapField(int fieldId, object value, ref T entity);
}