using DemLock.Utils;

namespace DemLock.Entities;

/// <summary>
/// Demo object that is the most atomic object that can be instantiated
/// as this can represent an entity or a base data type.
/// </summary>
public class DObject
{
    /// <summary>
    /// This will be called to set the value if it is an already decoded value
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetValue(object value)
    {
        
    }
    /// <summary>
    /// This will be used to decode the value from a bit stream if needed.
    /// There will be a callback function that is placed in the object at activation
    /// which will have all of the encoding information needed (just need to map that data
    /// to avoid costly closures)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bs"></param>
    public virtual void SetValue(ReadOnlySpan<int> path,BitStream bs)
    {
        
    }
    

}