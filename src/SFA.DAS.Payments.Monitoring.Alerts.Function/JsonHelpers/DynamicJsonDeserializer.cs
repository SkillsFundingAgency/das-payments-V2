using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using System.Text.Json;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers
{
    public class DynamicJsonDeserializer : IDynamicJsonDeserializer
    {
        public dynamic Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new ObjectAsPrimitiveConverter()
                },
                WriteIndented = true,
            };

            dynamic result = JsonSerializer.Deserialize<dynamic>(json, options);

            return result;
        }
    }
}