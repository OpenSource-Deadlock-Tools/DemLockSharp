using DemLock.Entities.FieldDecoders;

namespace DemLock.Entities;

public class EntityReader
{
    public string ClassName { get; set; }
    public List<string> Fieldnames { get; set; }
    public List<FieldDecoders.FieldDecoder> FieldDecoders { get; set; }

    public FieldDecoders.FieldDecoder ResolveDecoder(int[] fieldPath)
    {


        return new FieldDecoders.FieldDecoder();
    }
}

public class ReaderNode
{
    
}