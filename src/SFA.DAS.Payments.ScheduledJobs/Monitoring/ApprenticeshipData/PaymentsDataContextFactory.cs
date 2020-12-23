using System;
using Autofac;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface IPaymentsDataContextFactory
    {
        IPaymentsDataContext Create();
    }

    public class PaymentsDataContextFactory : IPaymentsDataContextFactory
    {
        private readonly ILifetimeScope lifetimeScope;

        public PaymentsDataContextFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public IPaymentsDataContext Create()
        {
            return lifetimeScope.Resolve<IPaymentsDataContext>();
        }
    }
}