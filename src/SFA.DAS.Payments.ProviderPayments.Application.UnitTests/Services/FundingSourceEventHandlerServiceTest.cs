using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Model.Enum;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class FundingSourceEventHandlerServiceTest
    {
        private FundingSourceEventHandlerService fundingSourceEventHandlerService;

        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;


        [OneTimeSetUp]
        public void SetUp()
        {
            providerPaymentsRepository = new Mock<IProviderPaymentsRepository>();
          
            providerPaymentsRepository
                .Setup(t => t.SavePayment(It.IsAny<PaymentDataEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        [Test]
        public async Task ProcessEventShouldCallRepository()
        {
            var fundingSourceEvent = new SfaCoInvestedFundingSourcePaymentEvent
            {
                ContractType = 2,
                FundingSourceType = FundingSourceType.CoInvestedSfa,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                CollectionPeriod = new CalendarPeriod(2018, 10),
                DeliveryPeriod = new CalendarPeriod(2018, 11),
                LearningAim = new LearningAim
                {
                    FrameworkCode = 1,
                    Reference = "100",
                    PathwayCode = 1,
                    StandardCode = 1,
                    ProgrammeType = 1,
                    AgreedPrice = 1000m,
                    FundingLineType = "T"
                },
                Learner = new Learner
                {
                    Ukprn = 100000,
                    ReferenceNumber = "A1000",
                    Uln = 10000000
                }
            };

            fundingSourceEventHandlerService = new FundingSourceEventHandlerService(providerPaymentsRepository.Object);

            await fundingSourceEventHandlerService.ProcessEvent(fundingSourceEvent, default(CancellationToken));

            providerPaymentsRepository
                .Verify(o => o.SavePayment(It.IsAny<PaymentDataEntity>(), default(CancellationToken)), Times.Once);

        }

    }
}
