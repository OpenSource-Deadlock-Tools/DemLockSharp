using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemLock.ClassMappingGenerator;

[JsonConverter(typeof(FieldDefinitionConverter))]
public class FieldDefinition
{
    public Dictionary<string, FieldDefinition>? Children { get; set; }
    public int? Path { get; set; }
    public string? Type { get; set; }
    public string? NetworkType { get; set; }
}

public class FieldDefinitionConverter : JsonConverter<FieldDefinition>
{
    public override void WriteJson(JsonWriter writer, FieldDefinition value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        if (value.Path != null)
        {
            writer.WritePropertyName("Path");
            writer.WriteValue(value.Path.Value);
        }

        if (value.Type != null)
        {
            writer.WritePropertyName("Type");
            writer.WriteValue(value.Type);
        }

        if (value.NetworkType != null)
        {
            writer.WritePropertyName("NetworkType");
            writer.WriteValue(value.NetworkType);
        }

        if (value.Children != null)
        {
            foreach (var entry in value.Children)
            {
                writer.WritePropertyName(entry.Key);
                WriteJson(writer, entry.Value, serializer);
            }
        }

        writer.WriteEndObject();
    }

    public override FieldDefinition ReadJson(JsonReader reader, Type objectType, FieldDefinition existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        return DeserializeFieldDefinition(jsonObject);
    }

    private FieldDefinition DeserializeFieldDefinition(JObject jsonObject)
    {
        var fieldDefinition = new FieldDefinition();
        foreach (var property in jsonObject.Properties())
        {
            // Consume the top level expected properties... rest we can assume are extending types
            if (property.Name == "Path" && property.Value is JValue{Type: JTokenType.Integer} fieldNumber)
            {
                fieldDefinition.Path = fieldNumber.Value<int>();
                continue;
            }

            if (property.Name == "Type" && property.Value is JValue{Type: JTokenType.String} fieldType)
            {
                fieldDefinition.Type = fieldType.Value<string>();
                continue;
            }

            if (property.Name == "NetworkType" && property.Value is JValue{Type: JTokenType.String} networkType)
            {
                fieldDefinition.NetworkType = networkType.Value<string>();
                continue;
            }

            // Make sure it's a JObject we are looking at
            if(property.Type != JTokenType.Object)
                continue;
            
            // Since we got here, we will make sure we only initialize the dictionary once
            if(fieldDefinition.Children == null)
                fieldDefinition.Children = new Dictionary<string, FieldDefinition>();
            
            fieldDefinition.Children[property.Name] = DeserializeFieldDefinition((JObject)property.Value);
        }


        return fieldDefinition;
    }

    private bool TryDeserializeProperty(JProperty property, out FieldDefinition fieldDefinition)
    {
        fieldDefinition = null;
        if (property.Value.Type != JTokenType.Object) return false;

        var childObject = (JObject)property.Value;
        if (childObject["Path"] == null || childObject["Type"] == null || childObject["NetworkType"] == null)
            return false;

        fieldDefinition = new FieldDefinition
        {
            Path = (int)childObject["Path"],
            Type = (string)childObject["Type"],
            NetworkType = (string)childObject["NetworkType"]
        };
        return true;
    }
}