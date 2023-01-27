namespace SFA.DAS.Payments.Monitoring.Alerts.Function.JsonHelpers
{
    public interface IDynamicJsonDeserializer
    {
        public dynamic Deserialize(string json);
    }
}