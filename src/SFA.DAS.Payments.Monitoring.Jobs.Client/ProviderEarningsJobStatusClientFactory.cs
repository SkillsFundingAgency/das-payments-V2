using System;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IProviderEarningsJobStatusClientFactory
    {
        IProviderEarningsJobStatusClient Create();
    }

    public class ProviderEarningsJobStatusClientFactory: IProviderEarningsJobStatusClientFactory
    {
        private readonly ILifetimeScope scope;

        public ProviderEarningsJobStatusClientFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
        public IProviderEarningsJobStatusClient Create()
        {
            return scope.Resolve<IProviderEarningsJobStatusClient>();
        }
    }
}