using System;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Infrastructure
{
    public class TestConfigurationHelper : IConfigurationHelper
    {
        private readonly TestsConfiguration config;

        public TestConfigurationHelper(TestsConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public bool HasSetting(string sectionName, string settingName)
        {
            return config.GetAppSetting(settingName) != null;
        }

        public string GetSetting(string sectionName, string settingName)
        {
            return config.GetAppSetting(settingName);
        }
    }
}