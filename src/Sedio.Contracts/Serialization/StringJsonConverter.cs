using System;
using Newtonsoft.Json;

namespace Sedio.Contracts.Serialization
{
    public abstract class StringJsonConverter<T> : JsonConverter<T>
    {
        private readonly bool isReferenceType = IsNullable(typeof(T));
        
        public override T ReadJson(JsonReader reader, 
                                   Type objectType, 
                                   T existingValue, 
                                   bool hasExistingValue, 
                                   JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                {
                    return OnReadNull();
                }
                case JsonToken.String:
                    
                    var stringValue = reader.Value.ToString();
                    
                    if (OnTryParse(stringValue, out var result))
                    {
                        return result;
                    }
                    
                    throw new JsonException($"Cannot parse {stringValue} into instance of type {typeof(T)}");
            }
            
            throw new JsonException($"{typeof(T)} must be a string in Json, found {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            if (isReferenceType)
            {
                if (value == null)
                {
                   OnWriteNull(writer,serializer);
                }
                else
                {
                    writer.WriteValue(OnToString(value));
                }
            }
            else
            {
                writer.WriteValue(OnToString(value));
            }
        }

        protected abstract bool OnTryParse(string value, out T result);

        protected virtual string? OnToString(T value)
        {
            return value?.ToString();
        }

        protected virtual void OnWriteNull(JsonWriter writer, JsonSerializer serializer)
        {
            writer.WriteNull();
        }

        protected virtual T OnReadNull()
        {
            if (isReferenceType)
            {
#nullable disable 
                return (T)(object)null;
#nullable enable
            }
                    
            throw new JsonException($"Type {typeof(T)} is a value type, cannot be null in JSON");
        }

        private static bool IsNullable(Type type)
        {
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                if (genericType == typeof(Nullable<>))
                {
                    return true;
                }
            }

            return !type.IsValueType;
        }
    }
}