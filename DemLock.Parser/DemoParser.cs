using DemLock.Parser.Events;
using DemLock.Parser.Models;
using DemLock.Utils;

namespace DemLock.Parser;

/// <summary>
/// The main demo parser that will be used to set up the event listeners, and then send in a file to be
/// processed
/// </summary>
public class DemoParser
{
    public NetMessages NetMessages { get; init; }
    public GameEvents GameEvents { get; init; }
    public UserMessages UserMessages { get; init; }
    public ServiceMessages ServiceMessages { get; init; }
    public PacketMessages PacketMessages { get; init; }
    public DemoFrames DemoFrames { get; init; }

    private FrameHandler _frameHandler;
    private MessageHandler _messageHandler;
    private DemoParserContext _context;
    private DemoParserConfig _config;

    public DemoParser(DemoParserConfig config)
    {
        _config = config;
        GameEvents = new GameEvents();
        ServiceMessages = new ServiceMessages();
        UserMessages = new UserMessages();
        NetMessages = new NetMessages();
        DemoFrames = new DemoFrames();
        PacketMessages = new PacketMessages();

        _context = new DemoParserContext(_config);
        _messageHandler = new MessageHandler( _context, GameEvents, NetMessages, UserMessages, ServiceMessages);
        _frameHandler = new FrameHandler( _messageHandler, _context);

        DemoFrames.OnPacket += packet =>
        {
            BitStream bs = new BitStream(packet.Data.ToByteArray());
            while (bs.BitsRemaining > 8)
            {
                var msgtype = (MessageTypes)bs.ReadUBit();
                var msgSize = bs.ReadVarUInt32();
                byte[] msgData = bs.ReadBytes(msgSize);
                PacketMessages.ProcessMessage(msgtype, msgData);
            }
        };

        DemoFrames.OnSignonPacket += packet =>
        {
            BitStream bs = new BitStream(packet.Data.ToByteArray());
            while (bs.BitsRemaining > 8)
            {
                var msgtype = (MessageTypes)bs.ReadUBit();
                var msgSize = bs.ReadVarUInt32();
                byte[] msgData = bs.ReadBytes(msgSize);
                PacketMessages.ProcessMessage(msgtype, msgData);
            }
        };

        DemoFrames.OnFullPacket += packet =>
        {
            BitStream bs = new BitStream(packet.Packet.Data.ToByteArray());
            while (bs.BitsRemaining > 8)
            {
                var msgtype = (MessageTypes)bs.ReadUBit();
                var msgSize = bs.ReadVarUInt32();
                byte[] msgData = bs.ReadBytes(msgSize);
                PacketMessages.ProcessMessage(msgtype, msgData);
            }
        };

        PacketMessages.OnNetMessage += NetMessages.HandleNetMessage;
        PacketMessages.OnGameEvent += GameEvents.HandleGameEventMessage;
        PacketMessages.OnServiceMessage += ServiceMessages.HandleServiceMessage;
        PacketMessages.OnUserMessage += UserMessages.HandleUserMessage;
    }

    /// <summary>
    /// Process a demo file, emitting events to any registered listeners when a derived event
    /// is calculated, which will contain data about the event (such as file info being parsed, 
    /// </summary>
    /// <param name="fileName"></param>
    public void ProcessDemo(string fileName)
    {
        // Make sure we clear our context to start fresh
        _context.ClearContext();
        using DemoStream demo = DemoStream.FromFilePath(fileName);
        FrameData frameData;
        int i = 0;
        do
        {
            frameData = demo.ReadFrame();
            _context.CurrentTick = frameData.Tick;
            DemoFrames.ProcessFrame(frameData);
            i++;
        } while (frameData.Command != DemoFrameCommand.DEM_Stop);
    }

    public void DumpClassDefinitions(string fileName, string outputDirectory)
    {
        // Make sure we clear our context to start fresh
        _context.ClearContext();
        using DemoStream demo = DemoStream.FromFilePath(fileName);
        FrameData frameData;
        int i = 0;
        do
        {
            frameData = demo.ReadFrame();
            _context.CurrentTick = frameData.Tick;
            if (frameData.Command == DemoFrameCommand.DEM_SendTables ||
                frameData.Command == DemoFrameCommand.DEM_ClassInfo)
            {
                _frameHandler.HandleFrame(frameData);
            }

            i++;
        } while (frameData.Command != DemoFrameCommand.DEM_Stop);

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }


        _context.DumpClassDefinitions(outputDirectory);
    }
}