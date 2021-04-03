using System;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.Configuration
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