using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;
using System;
using System.Threading;

namespace SFA.DAS.Payments.EarningEvents.EarningEventsService2122
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<EarningEventsService2122>())
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