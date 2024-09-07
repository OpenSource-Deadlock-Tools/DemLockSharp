﻿using DemLock.Entities;
using DemLock.Entities.DefinedObjects;
using DemLock.Entities.Primitives;

namespace DemLock.Parser.Models;

/// <summary>
/// Represents a field on a demo class, and is used in the
/// serializer to define how we are going to parse the entity
/// based on it's class type
/// </summary>
public class DField
{
    private DemoParserContext _context;

    public DField(DemoParserContext context)
    {
        _context = context;
    }

    public string? Name { get; set; }
    public DFieldType FieldType { get; set; }
    public string? SendNode { get; set; }
    public FieldEncodingInfo EncodingInfo { get; set; }
    public string SerializerName { get; set; }
    public int SerializerVersion { get; set; }

    private static int tmp = 0;

    public string PropertyType()
    {
        if (FieldType.Name == "CHandle")
            return nameof(Int32);

        if (FieldType.Count > 0 && FieldType.Name == "char")
            return nameof(String);

        // Manual override for simulation time data type, not sure why they don't have a type specified but this is how others do it
        if (Name == "m_flSimulationTime" || Name == "m_flAnimTime")
            return "float";

        if (FieldType.Count > 0)
            return $"{FieldType}[{FieldType.Count}] ";
        
        // If this has a generic, we will need to handle special cases
        if (FieldType.GenericType != null)
        {
            var childSerializer = _context.GetSerializerByClassName(FieldType.GenericType.Name);
            if (childSerializer == null)
                return $"List<{FieldType.GenericType.Name}>";

            return $"List<{childSerializer.Name}>";
        }
        // Generics will come with the serializer set to the serializer for their generic type
        // This will need to be packed and sen to the generic at some point to get generics working fully
        // For now we will just check that it's a generic to get through initial processing and come back to it later

        // If the serializer is named this is a nested entity we need to activate
        if (!string.IsNullOrWhiteSpace(SerializerName))
            return $"{SerializerName}";


        return $"{FieldType.Name}";
    }

    /// <summary>
    /// Get the activated field for this field template
    /// </summary>
    /// <returns></returns>
    public FieldDecoder Activate()
    {
        if (FieldType.Name == "CHandle")
            return new DUInt32();

        if (FieldType.Count > 0 && FieldType.Name == "char")
            return new DString(FieldType.Count);

        // Manual override for simulation time data type, not sure why they don't have a type specified but this is how others do it
        if (Name == "m_flSimulationTime" || Name == "m_flAnimTime")
            return new SimulationTime(_context.TickInterval);

        if (FieldType.Count > 0)
            return FieldDecoder.CreateFixedSizeArray(FieldType.Name, FieldType.Count,
                FieldDecoder.CreateObject(FieldType.Name, EncodingInfo));


        // If this has a generic, we will need to handle special cases
        if (FieldType.GenericType != null)
        {
            var childSerializer = _context.GetSerializerByClassName(FieldType.GenericType.Name);
            if (childSerializer == null)
                return FieldDecoder.CreateGenericObject(FieldType.Name, FieldType.GenericType.Name,
                    FieldDecoder.CreateObject(FieldType.GenericType.Name, EncodingInfo));

            return FieldDecoder.CreateGenericObject(FieldType.Name, FieldType.GenericType.Name,
                childSerializer.Instantiate());
        }
        // Generics will come with the serializer set to the serializer for their generic type
        // This will need to be packed and sen to the generic at some point to get generics working fully
        // For now we will just check that it's a generic to get through initial processing and come back to it later

        // If the serializer is named this is a nested entity we need to activate
        if (!string.IsNullOrWhiteSpace(SerializerName))
            return _context.GetSerializerByClassName(SerializerName, SerializerVersion).Instantiate();


        return FieldDecoder.CreateObject(FieldType.Name, EncodingInfo);
    }

    public override string ToString()
    {
        return $"{Name}::{SerializerName}";
    }
}