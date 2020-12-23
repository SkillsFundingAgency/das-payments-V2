using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests
{
    class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() =>
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());

                fixture.Register<ISubmissionJobsDataContext>(() => new InMemorySubmissionJobsDataContext());

                return fixture;
            })
        { }
    }
}
