using System;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IJobMessageClientFactory
    {
        IJobMessageClient Create();
    }

    public class JobMessageClientFactory : IJobMessageClientFactory
    {
        private readonly ILifetimeScope scope;

        public JobMessageClientFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public IJobMessageClient Create()
        {
            return scope.Resolve<IJobMessageClient>();
        }
    }
}