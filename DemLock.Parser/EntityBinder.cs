using DemLock.Parser.Events;

namespace DemLock.Parser;

public class EntityBinder
{
    public string EntityName { get; set; }
    public event EventHandler<List<EntityFieldData>> OnEntityChanged;

    public void RaiseEntityChangedEvent(List<EntityFieldData> entityFields)
    {
        OnEntityChanged?.Invoke(this, entityFields);
    }
}