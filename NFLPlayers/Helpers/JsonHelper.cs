using System.Text.Json;
namespace NFLPlayers.Helpers
{
    public static class JsonHelper
    {        
        public static string ExtTryGetStringPropertyCaseInsensitive(this JsonElement element, string propertyName)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value.GetString() ?? string.Empty;
                }
            }

            //throw new KeyNotFoundException($"Property '{propertyName}' not found.");
            return string.Empty;
        }

        public static int ExtTryGetInt32PropertyCaseInsensitive(this JsonElement element, string propertyName)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value.GetInt32();
                }
            }
            
            //throw new KeyNotFoundException($"Property '{propertyName}' not found.");
            return int.MinValue;
        
        }
    }
}

// Usage:
// JsonElement element = ... // Your JsonElement
// string name = GetPropertyCaseInsensitive(element, "name");  // This will work for "name", "Name", "NAME", etc.
