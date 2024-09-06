using DemLock.Parser;
using DemLock.Parser.Fields;

namespace DemLock;

public class CCitadelPlayerController
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public ulong SteamID { get; set; }
}

public class CustomPlayerMap : FieldMapping<CCitadelPlayerController>
{
    public override CCitadelPlayerController MapObject(List<EntityFieldData> fields)
    {
        throw new NotImplementedException();
    }
}