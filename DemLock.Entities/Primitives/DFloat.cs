namespace DemLock.Entities.Primitives;

/// <summary>
/// Represents a float32 in the entity space
/// </summary>
public class DFloat: DPrimitive
{
    /// <summary>
    /// The network name for the field type for checking what serializer to use
    /// </summary>
    public const string NetworkName = "float32";
    public float Value { get; set; }

    public DFloat(float value)
    {
        Value = value;
    }

}