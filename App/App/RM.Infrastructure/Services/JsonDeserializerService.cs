using System.Text.Json;
using App.RM.Application.Interfaces;

namespace App.RM.Infrastructure.Services;

public class JsonDeserializerService : IDeserializer
{
    public async Task<T?> DeserializeAsync<T>(Stream stream, JsonSerializerOptions options)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, options);
    }
}