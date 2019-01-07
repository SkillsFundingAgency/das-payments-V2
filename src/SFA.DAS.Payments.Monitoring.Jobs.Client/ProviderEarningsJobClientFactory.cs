using System;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IProviderEarningsJobClientFactory
    {
        IProviderEarningsJobClient Create();
    }

    public class ProviderEarningsJobClientFactory: IProviderEarningsJobClientFactory
    {
        private readonly ILifetimeScope scope;

        public ProviderEarningsJobClientFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
        public IProviderEarningsJobClient Create()
        {
            return scope.Resolve<IProviderEarningsJobClient>();
        }
    }
}