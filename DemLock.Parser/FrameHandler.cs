using DemLock.Entities;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

/// <summary>
/// Wrapper for all frame processing operations within the demo parsing process
/// </summary>
public class FrameHandler
{
    private readonly MessageHandler _messageHandler;
    private readonly DemoParserContext _context;

    public FrameHandler(MessageHandler messageHandler, DemoParserContext context)
    {
        _context = context;
        _messageHandler = messageHandler;
    }

    public void HandleFrame(FrameData frameData)
    {
        if(_context.Config.IgnoredFrames.Contains(frameData.Command)) return;
        
        switch (frameData.Command)
        {
            case DemoFrameCommand.DEM_Stop: return;
            case DemoFrameCommand.DEM_FileHeader:
                HandleFileHeader(frameData);
                break;
            case DemoFrameCommand.DEM_ClassInfo:
                HandleClassInfo(frameData);
                break;
            case DemoFrameCommand.DEM_SendTables:
                HandleSendTables(frameData);
                break;
            case DemoFrameCommand.DEM_Packet:
                HandlePacket(frameData);
                break;
            case DemoFrameCommand.DEM_SignonPacket:
                HandleSignonPacket(frameData);
                break;
            case DemoFrameCommand.DEM_FullPacket:
                HandleFullPacket(frameData);
                break;
        }
    }

    
    /// <summary>
    /// Handle a Full packet frame being received, this could likely be merged with handle packet,
    /// but for clarity wanted to keep them explicit
    /// </summary>
    /// <param name="frameData">The frame which contains the data to process</param>
    private void HandleFullPacket(FrameData frameData)
    {
        var packet = CDemoFullPacket.Parser.ParseFrom(frameData.Data);
        BitStream bs = new BitStream(packet.Packet.Data.ToByteArray());
        while (bs.BitsRemaining > 8)
        {
            var msgtype = (MessageTypes)bs.ReadUBit();
            var msgSize = bs.ReadVarUInt32();
            byte[] msgData = bs.ReadBytes(msgSize);
            _messageHandler.ProcessMessage(msgtype, msgData);
        }
    }
    /// <summary>
    /// Handle a signon packet frame ebing received, this could likely be merged with handle packet,
    /// but for clarity wanted to keep them explicit
    /// </summary>
    /// <param name="frameData">The frame which contains the data to process</param>
    private void HandleSignonPacket(FrameData frameData)
    {
        var packet = CDemoPacket.Parser.ParseFrom(frameData.Data);
        BitStream bs = new BitStream(packet.Data.ToByteArray());
        while (bs.BitsRemaining > 8)
        {
            var msgtype = (MessageTypes)bs.ReadUBit();
            var msgSize = bs.ReadVarUInt32();
            byte[] msgData = bs.ReadBytes(msgSize);
            _messageHandler.ProcessMessage(msgtype, msgData);
        }
    }
    
    private void HandleFileHeader(FrameData frameData)
    {
        var fileHeader = CDemoFileHeader.Parser.ParseFrom(frameData.Data);
    }

    private void HandlePacket(FrameData frameData)
    {
        var packet = CDemoPacket.Parser.ParseFrom(frameData.Data);
        BitStream bs = new BitStream(packet.Data.ToByteArray());
        while (bs.BitsRemaining > 8)
        {
            var msgtype = (MessageTypes)bs.ReadUBit();
            var msgSize = bs.ReadVarUInt32();
            byte[] msgData = bs.ReadBytes(msgSize);
            _messageHandler.ProcessMessage(msgtype, msgData);
        }
    }

    /// <summary>
    /// Handle the class info message frame and update the corresponding tables
    /// </summary>
    /// <param name="frameData"></param>
    private void HandleClassInfo(FrameData frameData)
    {
        CDemoClassInfo demClassInfo = CDemoClassInfo.Parser.ParseFrom(frameData.Data);

        foreach (var v in demClassInfo.Classes)
        {
            _context.AddClass(new DClass()
            {
                ClassId = v.ClassId,
                ClassName = v.NetworkName,
            });
        }
    }

    private void HandleSendTables(FrameData frameData)
    {
        CDemoSendTables sendTable = CDemoSendTables.Parser.ParseFrom(frameData.Data);

        BitBuffer bs = new BitBuffer(sendTable.Data.ToByteArray());
        
        Span<byte> byteArr = new Span<byte>(new byte[bs.ReadVarUInt32()]);
        bs.ReadBytes(byteArr);

        var msg = CSVCMsg_FlattenedSerializer.Parser.ParseFrom(byteArr);

        // Create the fields objects
        var symbols = msg.Symbols;
        var fields = msg.Fields.Select(field =>
        {
            DField newField = new DField(_context);
            newField.SerializerVersion = field.FieldSerializerVersion;
            newField.Name = symbols[field.VarNameSym];
            var fieldType = DFieldType.Parse(symbols[field.VarTypeSym]);
            newField.FieldType = fieldType;
            _context.AddFieldType(fieldType);
            newField.SendNode = symbols[field.SendNodeSym];
            var varEncoder = field.HasVarEncoderSym
                ? symbols[field.VarEncoderSym]
                : null;
            if (field.HasFieldSerializerNameSym)
                newField.SerializerName = symbols[field.FieldSerializerNameSym];
            else newField.SerializerName = string.Empty;
            newField.EncodingInfo = new FieldEncodingInfo()
            {
                VarEncoder = varEncoder,
                BitCount = field.BitCount,
                EncodeFlags = field.EncodeFlags,
                LowValue = field.HasLowValue ? field.LowValue : default(float?),
                HighValue = field.HasHighValue ? field.HighValue : default(float?)
            };
            _context.AddField(newField);
            return newField;
        }).ToArray();

        List<DSerializer> serializers = msg.Serializers
            .Select(sz =>
            {
                return new DSerializer()
                {
                    Name = msg.Symbols[sz.SerializerNameSym],
                    Version = sz.SerializerVersion,
                    Fields = sz.FieldsIndex.Select(i => fields[i]).ToArray()
                };
            }).ToList();
        _context.AddSerializerRange(serializers);
    }
}
