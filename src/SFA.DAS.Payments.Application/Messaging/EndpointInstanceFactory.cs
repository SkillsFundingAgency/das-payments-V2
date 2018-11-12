using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.Payments.Application.Messaging
{
    public interface IEndpointInstanceFactory
    {
        Task<IEndpointInstance> GetEndpointInstance();
    }

    public class EndpointInstanceFactory: IEndpointInstanceFactory
    {
        private readonly EndpointConfiguration endpointConfiguration;
        private static IEndpointInstance endpointInstance;
        //private static readonly SemaphoreSlim LockObject = new SemaphoreSlim(1, 1);
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        public EndpointInstanceFactory(EndpointConfiguration endpointConfiguration)
        {
            this.endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));
        }

        public async Task<IEndpointInstance> GetEndpointInstance()
        {
            //Locker.EnterUpgradeableReadLock();
            try
            {
                if (endpointInstance != null)
                    return endpointInstance;
                //Locker.EnterWriteLock();
                try
                {
                    endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
                }
                finally
                {
                    //Locker.ExitWriteLock();
                }
            }
            finally
            {
                //Locker.ExitUpgradeableReadLock();
            }
            return endpointInstance;
        }
    }
}