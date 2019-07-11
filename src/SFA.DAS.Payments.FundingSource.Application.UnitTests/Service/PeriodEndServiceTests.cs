using System.Threading.Tasks;
using Autofac.Extras.Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class PeriodEndServiceTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }

        [Test]
        public async Task Invokes_Period_End_For_Each_Employer()
        {
            var periodEndMessage = new PeriodEndRunningEvent
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1920,
                    Period = 10
                },
                JobId = 1234
            };
            var service = mocker.Create<PeriodEndService>();
            await service.RunPeriodEnd(periodEndMessage);
        }
    }
}