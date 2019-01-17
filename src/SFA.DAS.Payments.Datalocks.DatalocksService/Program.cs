using System;
using System.Threading;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

namespace SFA.DAS.Payments.Datalocks.DatalocksService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForActor<DatalocksService>())
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
