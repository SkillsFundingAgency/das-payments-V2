using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Services;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningEventsServiceTest
    {
        private IPaymentLogger logger;
        private Mock<IEarningEventMapper> mapper;
        private ApprenticeshipContractType2EarningEventsService service;

        [SetUp]
        public void Setup()
        {
            logger = Mock.Of<IPaymentLogger>();

            mapper =new Mock<IEarningEventMapper>();
            mapper.Setup(m => m.MapEarningEvent(It.IsAny<FM36Global>())).Verifiable();

            service = new ApprenticeshipContractType2EarningEventsService(logger, mapper.Object);
        }

        [Test]
        public void ShouldMapEarningEvents()
        {
            var fm36Output = new FM36Global();

            service.GetEarningEvents(fm36Output);

            mapper.VerifyAll();
        }
    }
}
