using System.Text.Json;

namespace App.Interfaces;

public interface IDeserializer
{
    Task<T?> DeserializeAsync<T>(Stream stream, JsonSerializerOptions options);
}