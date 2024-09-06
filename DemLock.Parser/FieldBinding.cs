namespace DemLock.Parser;

public class FieldBinding
{
    
}

public class FieldBindingBuilder<T>
{
    public FieldBindingBuilder<T> Field(string fieldPart)
    {
        return this;
    }

    public FieldBindingBuilder<T> MapTo<TKey>(Func<T, TKey> keySelector)
    {
        return this;
    }
    
    public FieldBindingBuilder<T> As<TObj>()
    {
        return this;
    }
    public FieldBindingBuilder<T> As<TObj>(string newName)
    {
        return this;
    }
}
public class FieldBindingBuilder
{
    public FieldBindingBuilder Field(string fieldPart)
    {
        return this;
    }

    public FieldBindingBuilder MapTo()
    {
        return this;
    }
    
    public FieldBindingBuilder As<T>()
    {
        return this;
    }
    public FieldBindingBuilder As<T>(string newName)
    {
        return this;
    }
}