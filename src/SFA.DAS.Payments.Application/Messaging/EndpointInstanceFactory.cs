using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;

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
        private static IStartableEndpointWithExternallyManagedContainer startableEndpoint;

        public EndpointInstanceFactory(EndpointConfiguration endpointConfiguration)
        {
            this.endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));
        }

        //TODO: Hack to cope with the new NSB config API.  Will refactor. 
        public static void Initialise(IStartableEndpointWithExternallyManagedContainer endpoint)
        {
            startableEndpoint = endpoint;
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
                    if (startableEndpoint == null)
                        throw new InvalidOperationException("EndpointInstanceFactory has not been initialised!!");
                    //var startableEndpoint =
                    //    EndpointWithExternallyManagedContainer.Create(endpointConfiguration, new ServiceCollection());

                    endpointInstance =
                        await startableEndpoint.Start(new AutofacServiceProvider(ContainerFactory.Container));
//                    endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
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