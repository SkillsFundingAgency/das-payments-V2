﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers
{
    public class ObjectAsPrimitiveConverter : JsonConverter<object>
    {
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    {
                        if (reader.TryGetInt32(out var i))
                            return i;
                        if (reader.TryGetInt64(out var l))
                            return l;
                        else if (reader.TryGetDouble(out var d))
                            return d;
                        using var doc = JsonDocument.ParseValue(ref reader);
                        return doc.RootElement.Clone();

                        throw new JsonException(string.Format("Cannot parse number {0}", doc.RootElement.ToString()));
                    }
                case JsonTokenType.StartArray:
                    {
                        var list = new List<object>();
                        while (reader.Read())
                        {
                            switch (reader.TokenType)
                            {
                                default:
                                    list.Add(Read(ref reader, typeof(object), options));
                                    break;
                                case JsonTokenType.EndArray:
                                    return list;
                            }
                        }
                        throw new JsonException();
                    }
                case JsonTokenType.StartObject:
                    var dict = CreateDictionary();
                    while (reader.Read())
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndObject:
                                return dict;
                            case JsonTokenType.PropertyName:
                                var key = reader.GetString();
                                reader.Read();
                                dict.Add(key, Read(ref reader, typeof(object), options));
                                break;
                            default:
                                throw new JsonException();
                        }
                    }
                    throw new JsonException();
                default:
                    throw new JsonException(string.Format("Unknown token {0}", reader.TokenType));
            }
        }

        protected virtual IDictionary<string, object> CreateDictionary() =>
            new ExpandoObject();
    }
}