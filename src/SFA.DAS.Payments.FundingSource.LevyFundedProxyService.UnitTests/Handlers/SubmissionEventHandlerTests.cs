using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers;
using SFA.DAS.Payments.FundingSource.LevyFundedService;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.UnitTests.Handlers
{
    internal class TestSubmissionEventHandler : SubmissionEventHandler<SubmissionJobFailed>
    {
        public TestSubmissionEventHandler(IActorProxyFactory proxyFactory, ILevyFundingSourceRepository repository,
            IPaymentLogger logger, IExecutionContext executionContext, ILevyMessageRoutingService levyMessageRoutingService) : base(proxyFactory, repository, logger, executionContext, levyMessageRoutingService)
        {
        }

        protected override async Task HandleSubmissionEvent(SubmissionJobFailed message, ILevyFundedService fundingService)
        {
            await fundingService.RemoveCurrentSubmission(message);
        }
    }

    [TestFixture]
    public class SubmissionEventHandlerTests
    {
        private TestSubmissionEventHandler _eventHandler;

        private Mock<ILevyFundingSourceRepository> _levyFundingSourceRepository;
        private Mock<ILevyMessageRoutingService> _levyMessageRoutingService;
        private Mock<IActorProxyFactory> _proxyFactory;

        private SubmissionJobFailed _event;

        [SetUp]
        public void SetUp()
        {
            _proxyFactory = new Mock<IActorProxyFactory>();
            _levyFundingSourceRepository = new Mock<ILevyFundingSourceRepository>();
            _levyMessageRoutingService = new Mock<ILevyMessageRoutingService>();

            var executionContext = new ESFA.DC.Logging.ExecutionContext();
            _eventHandler = new TestSubmissionEventHandler(_proxyFactory.Object, _levyFundingSourceRepository.Object, Mock.Of<IPaymentLogger>(), executionContext, _levyMessageRoutingService.Object);

            _event = new SubmissionJobFailed { Ukprn = 12344325 };
        }

        [Test]
        public async Task WhenAnAccountHasMultipleTransferSendersThenTheMessageIsRoutedToBoth()
        {
            var expectedAccounts = new List<Tuple<long, long?>> { new Tuple<long, long?>( 435345, 4354353 ), new Tuple<long, long?>( 45684532, 32497234 ) };
            _levyFundingSourceRepository.Setup(x => x.GetEmployerAccountsByUkprn(_event.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(expectedAccounts);

            _levyMessageRoutingService.Setup(x => x.GetDestinationAccountId(expectedAccounts[0].Item1, expectedAccounts[0].Item2)).Returns(expectedAccounts[0].Item2.Value);
            _levyMessageRoutingService.Setup(x => x.GetDestinationAccountId(expectedAccounts[1].Item1, expectedAccounts[1].Item2)).Returns(expectedAccounts[1].Item2.Value);

            var sender1LevyFundedService = new Mock<ILevyFundedService>();
            var sender2LevyFundedService = new Mock<ILevyFundedService>();

            _proxyFactory.Setup(x => x.CreateActorProxy<ILevyFundedService>(It.Is<Uri>(y => y.ToString() == LevyFundedServiceConstants.ServiceUri), It.Is<ActorId>(y => y.GetLongId() == expectedAccounts[0].Item2.Value), null)).Returns(sender1LevyFundedService.Object);
            _proxyFactory.Setup(x => x.CreateActorProxy<ILevyFundedService>(It.Is<Uri>(y => y.ToString() == LevyFundedServiceConstants.ServiceUri), It.Is<ActorId>(y => y.GetLongId() == expectedAccounts[1].Item2.Value), null)).Returns(sender2LevyFundedService.Object);

            await _eventHandler.Handle(_event, Mock.Of<IMessageHandlerContext>());

            sender1LevyFundedService.Verify(x => x.RemoveCurrentSubmission(_event), Times.Once);
            sender2LevyFundedService.Verify(x => x.RemoveCurrentSubmission(_event), Times.Once);
        }
    }
}
