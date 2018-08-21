using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                var builder = ServiceFabricContainerFactory.CreateBuilderForActor<RequiredPaymentsService>();
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