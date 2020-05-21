using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.EarningEvents.ProcessLearnerService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForActor<ProcessLearnerService>())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}
