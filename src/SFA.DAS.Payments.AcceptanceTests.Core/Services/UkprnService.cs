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

        public UkprnService(TestPaymentsDataContext dataContext, IJobService jobService)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.jobService = jobService;
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
                if (mutex.WaitOne(TimeSpan.FromMinutes(1)))
                {
                    provider = GetProvider();
                    // check job queue for ukprn - looking for status 2 or 3 which will block queue
                    var blockedList = jobService.GetJobsByStatus(provider.Ukprn, 2, 3).Result;
                    if (blockedList.Any())
                    {
                        provider = GetProvider();
                    }

                    dataContext.ClearPaymentsData(provider.Ukprn);
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
