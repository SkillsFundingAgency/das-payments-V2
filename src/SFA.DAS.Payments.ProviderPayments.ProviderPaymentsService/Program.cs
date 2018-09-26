using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;
using System;
using System.Threading;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForActor<ProviderPaymentsService>())
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
