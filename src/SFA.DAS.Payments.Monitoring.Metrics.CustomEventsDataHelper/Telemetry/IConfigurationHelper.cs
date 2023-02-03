namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry
{
    public interface IConfigurationHelper
    {
        bool HasSetting(string sectionName, string settingName);
        string GetSetting(string sectionName, string settingName);
    }
}