using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.TestUtilities
{
    class TimeService
    {
        private TimeSpan timeToWait = TimeSpan.FromSeconds(10);

        public TimeService()
        {
            var conf = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            if (TimeSpan.TryParse(conf["TimeToWait"], out var result))
            {
                timeToWait = result;
            }
        }

        public async Task<bool> WaitForBoolean(Func<Task<bool>> predicate, 
            bool expectedValue, 
            TimeSpan? timeToWait = null)
        {
            if (timeToWait == null)
            {
                timeToWait = this.timeToWait;
            }

            var endTime = DateTime.Now + timeToWait;
            while (DateTime.Now < endTime)
            {
                if (await predicate() == expectedValue)
                {
                    return expectedValue;
                }

                await Task.Delay(50);
            }

            return !expectedValue;
        }
    }
}
