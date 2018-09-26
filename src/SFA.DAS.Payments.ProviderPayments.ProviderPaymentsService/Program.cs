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
                var builder = ServiceFabricContainerFactory.CreateBuilderForActor<ProviderPaymentsService>();
                using (builder.Build())
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
