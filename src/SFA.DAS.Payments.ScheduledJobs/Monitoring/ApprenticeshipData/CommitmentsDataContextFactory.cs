using System;
using Autofac;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    public interface ICommitmentsDataContextFactory
    {
        ICommitmentsDataContext Create();
    }

    public class CommitmentsDataContextFactory : ICommitmentsDataContextFactory
    {
        private readonly ILifetimeScope lifetimeScope;

        public CommitmentsDataContextFactory(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public ICommitmentsDataContext Create()
        {
            return lifetimeScope.Resolve<ICommitmentsDataContext>();
        }
    }
}