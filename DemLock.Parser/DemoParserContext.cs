﻿using DemLock.Parser.Models;

namespace DemLock.Parser;

/// <summary>
/// Context object that will join together all of the data into a nice
/// and tidy interface so as to avoid reallocating objects on the heap
/// when they are needed for processing sharing data.
///
/// This is because there are a few tables we will need to keep track of the
/// entire time, and updates to one table can effect others, which is
/// really hard to track.
/// </summary>
public class DemoParserContext
{
    private List<DClass> _classes;
    private List<DEntity> _entities;
    private Dictionary<string, DSerializer> _serializers;

    private List<DFieldType> _fieldTypes;
    private List<DField> _fields;

    public DemoParserContext()
    {
        _classes = new List<DClass>();
        _entities = new List<DEntity>();
        _serializers = new Dictionary<string, DSerializer>();
        _fieldTypes = new List<DFieldType>();
        _fields = new List<DField>();
    }

    public void AddClass(DClass @class) => _classes.Add(@class);

    public DSerializer AddSerializer(DSerializer serializer)
    {
        _serializers[serializer.Name] = serializer;
        return serializer;
    }

    public void AddField(DField field)=> _fields.Add(field);

    public void AddFieldType(DFieldType fieldType)
    {
        if(!_fieldTypes.Contains(fieldType))
            _fieldTypes.Add(fieldType);
    } 

    public void ClearContext()
    {
        _classes.Clear();
        _entities.Clear();
        _serializers.Clear();
    }

    public void PrintClasses()
    {
        Console.WriteLine("Class Table--------");
        int i = 0;
        foreach (DClass @class in _classes)
        {
            Console.WriteLine($"[{i}] {@class.ClassId}::{@class.ClassName}");
            i++;
            if (i % 50 == 0) Console.ReadKey();
        }
    }

    public void PrintFields()
    {
        Console.WriteLine($"Fields--------[{_fields.Count}]");
        int i = 0;
        foreach (var field in _fields)
        {
            Console.WriteLine($"[{i}] {field.Name}::{field.SerializerName}::{field.SendNode}::{field.FieldType.Name}");
            i++;
            if (i % 50 == 0) Console.ReadKey();
        }
    }
    public void PrintFieldTypes()
    {
        Console.WriteLine("Field Types--------");
        int i = 0;
        foreach (var fieldType in _fieldTypes)
        {
            Console.WriteLine($"[{i}] {fieldType.Name}::{fieldType.Count}::{fieldType.IsPointer}");
            i++;
            if (i % 50 == 0) Console.ReadKey();
        }
    }
}