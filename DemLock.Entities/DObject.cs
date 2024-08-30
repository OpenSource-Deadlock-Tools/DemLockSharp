using DemLock.Entities.DefinedObjects;
using DemLock.Entities.FieldDecoders;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities;

/// <summary>
/// Demo object that is the most atomic object that can be instantiated
/// as this can represent an entity or a base data type.
/// </summary>
public abstract class DObject
{
    public bool IsSet { get; set; } = false;
    /// <summary>
    /// This will be called to set the value if it is an already decoded value
    /// </summary>
    /// <param name="value"></param>
    public abstract void SetValue(object value);
    /// <summary>
    /// This will be used to decode the value from a bit stream if needed.
    /// There will be a callback function that is placed in the object at activation
    /// which will have all of the encoding information needed (just need to map that data
    /// to avoid costly closures)
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bs"></param>
    public abstract void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs);

    public abstract object GetValue();

    public static DObject CreateObject(string typeName, FieldEncodingInfo fieldEncodingInfo)
    {
        if (typeName == "float32")
            return new DFloat();
        if(typeName == "uint16")
            return new DUInt16();
        if(typeName == "int16")
            return new DInt16();
        if (typeName == "CHandle")
            return new CHandle();
        if (typeName == "CNetworkedQuantizedFloat")
            return new CNetworkedQuantizedFloat(fieldEncodingInfo);
        if (typeName == "CGameSceneNodeHandle")
            return new CGameSceneNodeHandle();
        if (typeName == "QAngle")
            return new QAngle(fieldEncodingInfo);
        if (typeName == "CUtlStringToken")
            return new CUtlStringToken();

        return new DNull();
    }
}