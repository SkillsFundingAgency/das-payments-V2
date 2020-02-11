using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1,
    RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.Audit.EarningEventsService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatefulService<EarningEventsService>())
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
