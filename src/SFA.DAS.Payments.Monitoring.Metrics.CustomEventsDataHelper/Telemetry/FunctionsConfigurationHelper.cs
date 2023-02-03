using System;


namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry
{
    public class FunctionsConfigurationHelper : IConfigurationHelper
    {
        public bool HasSetting(string sectionName, string settingName)
        {
            return Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process).Contains(settingName);
        }

        public string GetSetting(string sectionName, string settingName)
        {
            return Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process);
        }
    }
}