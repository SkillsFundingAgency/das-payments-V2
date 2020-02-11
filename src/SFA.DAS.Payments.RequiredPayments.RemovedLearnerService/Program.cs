 using System;
 using System.Threading;
 using Microsoft.ServiceFabric.Services.Remoting;
 using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
 using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

 [assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1,
     RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.RequiredPayments.RemovedLearnerService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForActor<RemovedLearnerService>())
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
