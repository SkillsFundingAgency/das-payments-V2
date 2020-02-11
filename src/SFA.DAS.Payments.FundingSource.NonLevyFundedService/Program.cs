using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;
using System;
using System.Threading;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1,
    RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForStatelessService<NonLevyFundedService>())
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