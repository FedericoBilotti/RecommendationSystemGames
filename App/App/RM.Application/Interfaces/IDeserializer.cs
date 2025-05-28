using System.Text.Json;

namespace App.RM.Application.Interfaces;

public interface IDeserializer
{
    Task<T?> DeserializeAsync<T>(Stream stream, JsonSerializerOptions options);
}