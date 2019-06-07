using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Integration.ServiceFabric;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint
{
    public class TestEndpoint : StatelessService
    {
        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private IStatelessEndpointCommunicationListener listener;

        public TestEndpoint(StatelessServiceContext context, 
            ILifetimeScope lifetimeScope, 
            IPaymentsDataContext paymentsDataContext,
            IEndpointInstanceFactory endpointInstanceFactory): base(context)
        {
            this.paymentsDataContext = paymentsDataContext;
            this.lifetimeScope = lifetimeScope;
            this.endpointInstanceFactory = endpointInstanceFactory;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, remotingListener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");
                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(services =>
                                    {
                                        services.AddSingleton<IEndpointInstanceFactory>(endpointInstanceFactory);
                                        services.AddSingleton<IPaymentsDataContext>(paymentsDataContext);
                                        services.AddSingleton<StatelessServiceContext>(serviceContext);
                                    })
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(remotingListener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }),name: "KestrelCommunicationListener"),

                new ServiceInstanceListener(context => 
                    listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>(), 
                    name: "ServiceBusCommunicationListener")

            };
        }
    }
}
