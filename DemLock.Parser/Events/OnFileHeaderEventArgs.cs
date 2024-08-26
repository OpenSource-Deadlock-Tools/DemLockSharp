namespace DemLock.Parser.Events;

public class OnFileHeaderEventArgs: BaseDemoEventArgs
{
    public string DemoFileStamp { get; set; }
    public int NetworkProtocol { get; set; }
    public string ServerName { get; set; }
    public string ClientName { get; set; }
    public string MapName { get; set; }
    public string GameDirectory { get; set; }
    public int FullpacketsVersion { get; set; }
    public bool AllowClientsideEntities { get; set; }
    public bool AllowClientsideParticles { get; set; }
    public string Addons { get; set; }
    public string DemoVersionName { get; set; }
    public string DemoVersionGuid { get; set; }
    public int BuildNum { get; set; }
    public string Game { get; set; }
    public int ServerStartTick { get; set; }

    
    internal OnFileHeaderEventArgs(CDemoFileHeader header, uint tick): base(tick)
    {
        MapHeader(header);
    }
    internal OnFileHeaderEventArgs(CDemoFileHeader header):base (0)
    {
        MapHeader(header);
    }

    private void MapHeader(CDemoFileHeader header)
    {
        DemoFileStamp = header.DemoFileStamp;
        NetworkProtocol = header.NetworkProtocol;
        ServerName = header.ServerName;
        ClientName = header.ClientName;
        MapName = header.MapName;
        GameDirectory = header.GameDirectory;
        FullpacketsVersion = header.FullpacketsVersion;
        AllowClientsideEntities = header.AllowClientsideEntities;
        AllowClientsideParticles = header.AllowClientsideParticles;
        Addons = header.Addons;
        DemoVersionName = header.DemoVersionName;
        DemoVersionGuid = header.DemoVersionGuid;
        BuildNum = header.BuildNum;
        Game = header.Game;
        ServerStartTick = header.ServerStartTick;
    }
}