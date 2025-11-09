using System.Text;
using System.Text.Json;

namespace Waggle.Common.Pagination.Cursors
{
    public static class CursorHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// Encodes a dictionary of values into a base64 cursor string.
        /// </summary>
        public static string Encode(Dictionary<string, object?> values)
        {
            var data = new CursorData { Values = values };
            var json = JsonSerializer.Serialize(data, JsonOptions);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        /// <summary>
        /// Decodes a base64 cursor string into a dictionary of values.
        /// Returns null if input is invalid.
        /// Properly converts JsonElement objects to their actual types.
        /// </summary>
        public static Dictionary<string, object?>? Decode(string? cursor)
        {
            if (string.IsNullOrEmpty(cursor)) return null;

            try
            {
                var bytes = Convert.FromBase64String(cursor);
                var json = Encoding.UTF8.GetString(bytes);
                var data = JsonSerializer.Deserialize<CursorData>(json, JsonOptions);

                if (data?.Values == null) return null;

                // Convert JsonElement values to actual types
                var result = new Dictionary<string, object?>();
                foreach (var kvp in data.Values)
                {
                    result[kvp.Key] = ConvertJsonElement(kvp.Value);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a JsonElement to its actual underlying type.
        /// </summary>
        private static object? ConvertJsonElement(object? value)
        {
            if (value is not JsonElement element)
                return value;

            return element.ValueKind switch
            {
                JsonValueKind.String => TryParseStringValue(element.GetString()),
                JsonValueKind.Number => TryParseNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => value
            };
        }

        /// <summary>
        /// Attempts to parse string values into DateTime or Guid, otherwise returns the string.
        /// </summary>
        private static object TryParseStringValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return value ?? string.Empty;

            // Try parsing as DateTime (ISO 8601 format)
            if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dateTime))
                return dateTime;

            // Try parsing as Guid
            if (Guid.TryParse(value, out var guid))
                return guid;

            // Return as string if no special parsing applies
            return value;
        }

        /// <summary>
        /// Attempts to parse JSON numbers into appropriate numeric types.
        /// </summary>
        private static object TryParseNumber(JsonElement element)
        {
            // Try integer first
            if (element.TryGetInt32(out var intValue))
                return intValue;

            // Try long
            if (element.TryGetInt64(out var longValue))
                return longValue;

            // Try double
            if (element.TryGetDouble(out var doubleValue))
                return doubleValue;

            // Fallback to decimal
            return element.GetDecimal();
        }
    }
}