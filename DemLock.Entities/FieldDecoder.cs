using System.Text.Json.Nodes;
using DemLock.Entities.DefinedObjects;
using DemLock.Entities.Generics;
using DemLock.Entities.Primitives;
using DemLock.Utils;

namespace DemLock.Entities;

/// <summary>
/// Demo object that is the most atomic object that can be instantiated
/// as this can represent an entity or a base data type.
/// </summary>
public abstract class FieldDecoder
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
    public abstract object SetValue(ReadOnlySpan<int> path, ref BitBuffer bs);

    public virtual FieldDecoder GetFieldDecoder(ReadOnlySpan<int> path)
    {
        return this;
    }
    public abstract object ReadValue(ref BitBuffer bs);

    public virtual void ReadFieldName(ReadOnlySpan<int> path, ref string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
            fieldName = string.Empty;
    }
    public static FieldDecoder CreateFixedSizeArray(string typeName, int count, FieldDecoder childDecoder)
    {
        return new DFixedSizeArray(typeName, count, childDecoder);
    }
    /// <summary>
    /// Create a new object that's a generic, this is segmented as there will need to be some special handling for generics
    /// and it can easily be detected in the activator
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="genericTypeName"></param>
    /// <returns></returns>
    public static FieldDecoder CreateGenericObject(string typeName, string genericTypeName, FieldDecoder childDecoder)
    {
        if(typeName == "CNetworkUtlVectorBase")
            return new CNetworkUtlVectorBase(genericTypeName, childDecoder);
        if (typeName == "CHandle")
            return new CHandle(genericTypeName);
        if(typeName == "CStrongHandle")
            return new CStrongHandle(genericTypeName);
        if (typeName == "CUtlVector")
            return new CUtlVector(genericTypeName, childDecoder);
        if(typeName == "CUtlVectorEmbeddedNetworkVar")
            return new CUtlVectorEmbeddedNetworkVar(genericTypeName, childDecoder);

        throw new Exception($"Unmapped generic type {typeName}");
        return new DNull();
    }
    public static FieldDecoder CreateObject(string typeName, FieldEncodingInfo fieldEncodingInfo)
    {
        if (typeName == "float32")
            return new DFloat(fieldEncodingInfo);
        if(typeName == "uint16")
            return new DUInt16();
        if(typeName == "int16")
            return new DInt16();
        if (typeName == "CNetworkedQuantizedFloat")
            return new CNetworkedQuantizedFloat(fieldEncodingInfo);
        if (typeName == "CGameSceneNodeHandle")
            return new CGameSceneNodeHandle();
        if (typeName == "QAngle")
            return new QAngleDecoder(fieldEncodingInfo);
        if (typeName == "CUtlStringToken")
            return new CUtlStringToken();
        if (typeName == "bool")
            return new DBool();
        if (typeName == "uint64")
            return new DUInt64(fieldEncodingInfo);
        if (typeName == "int8")
            return new DInt8();
        if(typeName == "uint8")
            return new DUInt8();
        if (typeName == "int32")
            return new DInt32();
        if (typeName == "uint32")
            return new DUInt32();

        if (typeName == "GameTime_t")
            return new GameTime();
        if (typeName == "Color")
            return new DColor();
        if (typeName == "Vector")
            return new Vector(fieldEncodingInfo);

        // Need to handle
        if (typeName == "Vector2D")
            return new Vector2D(fieldEncodingInfo);
        if (typeName == "Vector4D")
            return new Vector4D(fieldEncodingInfo);
        
        if(typeName == "HSequence")
            return new HSequence();

        if (typeName == "CUtlSymbolLarge")
            return new CUtlSymbolLarge();
        if (typeName == "CUtlString")
            return new CUtlString();
        
        // Default to a UInt32 (basically just do what visit_ident is doing in haste)
        return new DUInt32();
    }
}