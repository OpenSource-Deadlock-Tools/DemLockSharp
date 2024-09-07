using DemLock.Utils;

namespace DemLock.Entities;

public record struct ActiveField(string FieldName, FieldDecoder Value);