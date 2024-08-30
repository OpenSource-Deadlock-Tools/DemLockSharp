using DemLock.Utils;

namespace DemLock.Entities;

public class ActiveField
{
    public int Index { get; set; }
    public string Name { get; set; }

    public void Decode(BitStream bs)
    {
        
    }
    
}