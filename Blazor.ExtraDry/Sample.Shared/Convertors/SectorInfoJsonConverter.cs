﻿#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sample.Shared.Converters {

    /// <summary>
    /// Converts a subset of a `Sector` for use when `Sector` is a child of another object.
    /// </summary>
    public class SectorInfoJsonConverter : JsonConverter<Sector> {
        public override Sector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType != JsonTokenType.StartObject) {
                throw new JsonException();
            }
            var sector = new Sector();
            var comparison = options.PropertyNameCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            while(reader.Read()) {
               if(reader.TokenType == JsonTokenType.PropertyName) {
                    var property = reader.GetString();
                    reader.Read();
                    if(reader.TokenType == JsonTokenType.String) {
                        var value = reader.GetString();
                        if(nameof(Sector.UniqueId).Equals(property, comparison)) {
                            sector.UniqueId = Guid.Parse(value);
                        }
                        else if(nameof(Sector.Title).Equals(property, comparison)) {
                            sector.Title = value;
                        }
                    }
                    else {
                        throw new JsonException();
                    }
                }
                else if(reader.TokenType == JsonTokenType.EndObject) {
                    return sector;
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Sector sector, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(Renamer(nameof(sector.UniqueId)), sector.UniqueId);
            writer.WriteString(Renamer(nameof(sector.Title)), sector.Title);
            writer.WriteEndObject();

            string Renamer(string name) => options.PropertyNamingPolicy?.ConvertName(name) ?? name;
        }
        
    }
}
