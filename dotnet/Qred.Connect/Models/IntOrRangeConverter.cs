using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Qred.Connect
{
  public class IntOrRangeConverter : JsonConverter
    {
      public override bool CanConvert(Type objectType) =>
        typeof(IntOrRange) == objectType;

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
        if (objectType!=typeof(IntOrRange)) return serializer.Deserialize(reader);
        do
        {
          var startToken = reader.TokenType;

          switch (startToken)
          {
            case JsonToken.Comment:
            case JsonToken.EndObject:
              reader.Skip();
              break;
            case JsonToken.Integer:
              return IntOrRange.Create((long)reader.Value);
            case JsonToken.Null:
              return null;
            case JsonToken.StartArray:
              return ExpectArray(reader);
            default: throw new JsonReaderException(
                $"Unrecognized token: {reader.TokenType}");
          }
        } while (reader.Read());
        return null;
      }
      private object ExpectArray(JsonReader reader)
      {
        var array = new List<long?>();
        while (reader.Read())
        {
          switch (reader.TokenType)
          {
            case JsonToken.Integer:
              array.Add((long)reader.Value);
              break;
            case JsonToken.Null:
              array.Add(null);
              break;
            case JsonToken.Comment:
              break;
            case JsonToken.EndArray:
              return ToIntOrRange(array.ToArray());
            default:
              throw new JsonReaderException(
                $"Unrecognized token: {reader.TokenType}");
          }
        }
        throw new JsonReaderException(
                $"Expected end of array: {reader.TokenType}");
      }

      private object ToIntOrRange(long?[] v)
      {
        if (v.Length != 2)
        {
          throw new JsonReaderException(
              $"Expected an array of length 2");
        }
        if (v[0] == null)
        {
          throw new JsonReaderException("Array: Expected first value to be not null");
        }
        return IntOrRange.Create(v[0].Value, v[1]);
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        if (ReferenceEquals(null, value))
        {
          writer.WriteNull();
        }
        else
        {
          var v = (IntOrRange)value;
          v.Match(singleValue =>
          {
            writer.WriteValue(singleValue);
          }, (from, to) =>
          {
            writer.WriteStartArray();
            writer.WriteValue(from);
            if (to != null)
            {
              writer.WriteValue(to.Value);
            }
            else
            {
              writer.WriteNull();
            }
            writer.WriteEndArray();
          });
        }
      }
    }
}