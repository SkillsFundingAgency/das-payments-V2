using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;
using Provider = SFA.DAS.Payments.AcceptanceTests.Core.TestModels.Provider;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    internal class UkprnService : IUkprnService
    {
        private readonly TestPaymentsDataContext dataContext;
        private readonly IJobService jobService;
        private readonly IApprenticeshipEarningsHistoryService appEarnHistoryService;

        public UkprnService(TestPaymentsDataContext dataContext, IJobService jobService, IApprenticeshipEarningsHistoryService appEarnHistoryService)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.jobService = jobService;
            this.appEarnHistoryService = appEarnHistoryService;
        }

        public int GenerateUkprn()
        {
            string appGuid =
                ((GuidAttribute)Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(GuidAttribute), false).
                    GetValue(0)).Value.ToString();

            Provider provider = null;
            using (var mutex = new Mutex(false, $"Global\\{{{appGuid}}}"))
            {
                if (mutex.WaitOne(TimeSpan.FromMinutes(5)))
                {
                    var attempts = 0;
                    while (true)
                    {
                        provider = GetProvider();
                        var providerBusy = jobService.IsProviderInAnActiveJob(provider.Ukprn).Result;
                        if (!providerBusy)
                            break;

                        attempts++;

                        if (attempts >= 10)
                            throw new ApplicationException("Could not find an unused provider after 10 attempts");
                    }
                    

                    dataContext.ClearPaymentsData(provider.Ukprn);
                    appEarnHistoryService.DeleteHistory(provider.Ukprn);
                    mutex.ReleaseMutex();
                }
                else
                {
                    throw new ApplicationException("Unable to obtain a Ukprn due to a locked Mutex");
                }
            }

            return provider.Ukprn;
        }

        private Provider GetProvider()
        {
            var provider = dataContext.LeastRecentlyUsed();
            provider.Use();
            dataContext.SaveChanges();
            return provider;
        }
    }
}
