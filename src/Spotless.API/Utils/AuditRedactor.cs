using System.Text.Json;
using System.Text.Json.Nodes;

namespace Spotless.API.Utils
{
    public static class AuditRedactor
    {
        private static readonly HashSet<string> DefaultSensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "password",
            "passwordConfirmation",
            "cardNumber",
            "card_number",
            "card",
            "cvv",
            "cvc",
            "ssn",
            "pan",
            "securityCode",
            "cardToken",
            "apiKey",
            "secret",
            "webhookSecret"
        };

        public static string RedactJson(string json, IEnumerable<string>? additionalKeys = null)
        {
            if (string.IsNullOrWhiteSpace(json)) return json;

            try
            {
                var node = JsonNode.Parse(json);
                if (node == null) return json;

                var keys = new HashSet<string>(DefaultSensitiveKeys, StringComparer.OrdinalIgnoreCase);
                if (additionalKeys != null)
                {
                    foreach (var k in additionalKeys) keys.Add(k);
                }

                RedactNode(node, keys);

                return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
            }
            catch
            {

                var redacted = json;
                foreach (var k in DefaultSensitiveKeys)
                {
                    redacted = redacted.Replace($"\"{k}\"", $"\"{k}\":\"***REDACTED***\"", StringComparison.OrdinalIgnoreCase);
                }
                return redacted;
            }
        }

        private static void RedactNode(JsonNode node, HashSet<string> keys)
        {
            if (node is JsonObject obj)
            {
                var props = obj.ToList();
                foreach (var kv in props)
                {
                    if (kv.Value == null) continue;
                    if (keys.Contains(kv.Key))
                    {
                        obj[kv.Key] = "***REDACTED***";
                    }
                    else
                    {
                        RedactNode(kv.Value, keys);
                    }
                }
            }
            else if (node is JsonArray arr)
            {
                for (int i = 0; i < arr.Count; i++)
                {
                    var item = arr[i];
                    if (item != null) RedactNode(item, keys);
                }
            }
        }
    }
}
