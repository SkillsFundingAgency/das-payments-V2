using System;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client
{
    public interface IEarningsJobClientFactory
    {
        IEarningsJobClient Create();
    }

    public class EarningsJobClientFactory: IEarningsJobClientFactory
    {
        private readonly ILifetimeScope scope;

        public EarningsJobClientFactory(ILifetimeScope scope)
        {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
        public IEarningsJobClient Create()
        {
            return scope.Resolve<IEarningsJobClient>();
        }
    }
}