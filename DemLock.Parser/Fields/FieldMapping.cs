namespace DemLock.Parser.Fields;

public abstract class MappedEntity<TResult>
{
    public abstract void BindFields(List<EntityFieldData> fields, ref TResult target);

}