﻿using System.Text;
using DemLock.Utils;

namespace DemLock.Entities.DefinedObjects;

public class DFixedSizeArray:DObject
{
    public string TypeName { get; set; }
    public int Length { get; set; }
    private Func<DObject> _objectFactory;
    /// <summary>
    /// Right now this is a byte array but this is very very incorrect and I just need to get the baseline parsed and I can
    /// then properly address this issue
    /// </summary>
    public DObject[] Data { get; set; }
    public DFixedSizeArray(string typeName, int length, Func<DObject> objectFactory)
    {
        _objectFactory = objectFactory;
        TypeName = typeName;
        Length = length;
        Data = new DObject[length];
    }
    public override void SetValue(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(ReadOnlySpan<int> path, ref BitBuffer bs)
    {
        Console.WriteLine("Set fixed size array value");
        if(Data[path[0]] == null) Data[path[0]] = _objectFactory();
        Data[path[0]].SetValue(path[1..], ref bs);
    }

    public override string ToJson()
    {
        StringBuilder sb = new();
        sb.AppendLine("{");
        sb.AppendLine($"\"@TypeName\": \"{TypeName}\",");
        sb.AppendLine($"\"Length\": \"{Length}\"");
        sb.AppendLine("}");

        return sb.ToString();
    }
    public override object GetValue()
    {
        throw new NotImplementedException();
    }
}