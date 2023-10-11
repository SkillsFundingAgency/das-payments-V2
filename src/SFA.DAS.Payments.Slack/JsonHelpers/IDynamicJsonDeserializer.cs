namespace SFA.DAS.Payments.Slack.JsonHelpers
{
    public interface IDynamicJsonDeserializer
    {
        public dynamic Deserialize(string json);
    }
}