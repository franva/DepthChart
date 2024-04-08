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
            
            return int.MinValue;
        
        }
    }
}
