using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

/// <summary>
/// Wrapper for all frame processing operations within the demo parsing process
/// </summary>
public class FrameHandler
{
    private readonly DemoEventSystem _events;
    private readonly MessageHandler _messageHandler;
    private readonly DemoParserContext _context;

    public FrameHandler(DemoEventSystem eventSystem, MessageHandler messageHandler, DemoParserContext context)
    {
        _context = context;
        _events = eventSystem;
        _messageHandler = messageHandler;
    }

    public void HandleFrame(DemoFrame frame)
    {
        switch (frame.Command)
        {
            case DemoFrameCommand.DEM_Stop: return;
            case DemoFrameCommand.DEM_FileHeader:
                HandleFileHeader(frame);
                break;
            case DemoFrameCommand.DEM_ClassInfo:
                HandleClassInfo(frame);
                break;
            case DemoFrameCommand.DEM_SendTables:
                HandleSendTables(frame);
                break;
        }
    }

    private void HandleFileHeader(DemoFrame frame)
    {
        var fileHeader = CDemoFileHeader.Parser.ParseFrom(frame.Data);
        _events.RaiseOnFileHeader(frame.Tick, fileHeader);
    }

    /// <summary>
    /// Handle the class info message frame and update the corresponding tables
    /// </summary>
    /// <param name="frame"></param>
    private void HandleClassInfo(DemoFrame frame)
    {
        CDemoClassInfo demClassInfo = CDemoClassInfo.Parser.ParseFrom(frame.Data);

        foreach (var v in demClassInfo.Classes)
        {
            _context.AddClass(new DClass()
            {
                ClassId = v.ClassId,
                ClassName = v.NetworkName
            });
        }
    }

    private void HandleSendTables(DemoFrame frame)
    {
        CDemoSendTables sendTable = CDemoSendTables.Parser.ParseFrom(frame.Data);

        BitStream bs = new BitStream(sendTable.Data.ToByteArray());
        byte[] buf = bs.ReadBytes(bs.ReadVarUInt32());

        var msg = CSVCMsg_FlattenedSerializer.Parser.ParseFrom(buf);

        // Create the fields objects
        var symbols = msg.Symbols;
        foreach (var field in msg.Fields)
        {
            DField newField = new DField();
            
            newField.Name = symbols[field.VarNameSym];
            var fieldType = DFieldType.Parse(symbols[field.VarTypeSym]);
            newField.FieldType = fieldType;
            _context.AddFieldType(fieldType);
            newField.SendNode = symbols[field.SendNodeSym];
            
            var varEncoder = field.HasVarEncoderSym
                ? symbols[field.VarEncoderSym]
                : null;
            newField.SerializerName = symbols[field.FieldSerializerNameSym];
            newField.EncodingInfo = new DFieldEncodingInfo()
            {
                VarEncoder= varEncoder,
                BitCount= field.BitCount,
                EncodeFlags= field.EncodeFlags,
                LowValue= field.HasLowValue ? field.LowValue : default(float?),
                HighValue= field.HasHighValue ? field.HighValue : default(float?)
            };

            //var polymorphicTypes = field.PolymorphicTypes.Select(
            //    polymorphicField => new SerializerKey(
            //        symbols[polymorphicField.PolymorphicFieldSerializerNameSym],
            //        polymorphicField.PolymorphicFieldSerializerVersion))
            //    .ToArray();
            var varSerializer = field.HasVarSerializerSym
                ? symbols[field.VarSerializerSym]
                : null;
            
            _context.AddField(newField);
        }
        

        return;
        
        

        foreach (var flatSerializer in msg.Serializers)
        {
            DSerializer serializer = _context.AddSerializer(new DSerializer()
            {
                Name = msg.Symbols[flatSerializer.SerializerNameSym],
                Version = flatSerializer.SerializerVersion,
                Fields = new List<DField>()
            });

            foreach (var i in flatSerializer.FieldsIndex)
            {
            }
        }
    }
}