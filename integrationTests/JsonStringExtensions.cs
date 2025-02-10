using System.Text.Json;

namespace integrationTests;

public static class JsonStringExtensions
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    
    public static string ToIndentedJson(this string value)
    {
        return JsonSerializer.Serialize(JsonDocument.Parse(value), Options);
    }
}