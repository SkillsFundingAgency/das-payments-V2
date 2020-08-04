using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.Monitoring.Metrics.PeriodEndService
{
    internal static class Program
    {
            public static void Main()
            {
                try
                {
                    using (ServiceFabricContainerFactory.CreateContainerForStatelessService<PeriodEndService>())
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                }
                catch (Exception e)
                {
                    ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                    throw;
                }
            }
    }
}
