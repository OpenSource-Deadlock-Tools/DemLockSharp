namespace DemLock.Parser;

public class EntityBinding
{
    public string EntityName { get; set; }
    public event EventHandler<List<EntityFieldData>> OnEntityChanged;
    public Dictionary<string, FieldBinding> FieldBindings { get; set; } = new();

    public void RaiseEntityChangedEvent(List<EntityFieldData> entityFields)
    {
        OnEntityChanged?.Invoke(this, entityFields);
    }
}

public class EntityBindingBuilder<T>
{
    public void OnUpdate(Action<T> update)
    {
        
    }
    public EntityBindingBuilder<T> Entity(string entityName)
    {

        return this;
    }

    public EntityBindingBuilder<T> WithFields(Action<FieldBindingBuilder<T>> fields)
    {

        return this;
    }

    public T FilteredBind(Func<T,bool> predicate)
    {
        return default;
    }
    
}
public class EntityBindingBuilder
{
    public void OnUpdate(Action<Dictionary<string,object>> update)
    {
        
    }
    public EntityBindingBuilder Entity(string entityName)
    {

        return this;
    }

    public EntityBindingBuilder WithFields(Action<FieldBindingBuilder> fields)
    {

        return this;
    }
    
}