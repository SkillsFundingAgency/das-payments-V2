using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService.Handlers;
using SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueService.Interfaces;
using SFA.DAS.Payments.PaymentsDue.Domain.Enums;
using SFA.DAS.Payments.PaymentsDue.Domain.Interfaces;
using SFA.DAS.Payments.PaymentsDue.Messages.Entities;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.UnitTests.Test
{
    [TestFixture]
    public class PayableEarningEventHandlerTest
    {
        [Test]
        public async Task TestHandle()
        {
            // arrange
            IPayableEarningEvent earning = new PayableEarningEvent
            {
                Ukprn = 1,
                LearnRefNumber = "2",
                ContractType = ContractType.Act2,
                Learner = new LearnerEntity(),
                LearnAim = new LearnAimEntity
                {
                    ProgrammeType = 3,
                    FrameworkCode = 4,
                    PathwayCode = 5,
                    StandardCode = 6,
                    LearnAimRef = "7"
                },
                PriceEpisodes = new[]
                {
                    new PriceEpisodeEntity
                    {
                        StartDate = DateTime.Today.AddMonths(-1),
                        EndDate = DateTime.Today,
                        Periods = new byte[]
                        {
                            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
                        },
                        Price = 111
                    }
                }
            };

            var apprenticeshipKeyServiceMock = new Mock<IApprenticeshipKeyService>(MockBehavior.Strict);
            apprenticeshipKeyServiceMock.Setup(s => s.GenerateKey(earning.Ukprn, earning.LearnRefNumber, earning.LearnAim.FrameworkCode, earning.LearnAim.PathwayCode, (ProgrammeType)earning.LearnAim.ProgrammeType, earning.LearnAim.StandardCode, earning.LearnAim.LearnAimRef))
                .Returns("key")
                .Verifiable();

            var endpoint = new Mock<IEndpointCommunicationSender<IPaymentsDueEvent>>(MockBehavior.Strict);
            endpoint.Setup(e => e.Send(It.IsAny<IPaymentsDueEvent>())).Returns(Task.FromResult(0)).Verifiable();

            IEnumerable<ICalculatedPaymentDueEvent> paymentsDueEvents = new[]
            {
                new CalculatedPaymentDueEvent {PaymentDueEntity = new PaymentDueEntity()},
                new CalculatedPaymentDueEvent {PaymentDueEntity = new PaymentDueEntity()},
                new CalculatedPaymentDueEvent {PaymentDueEntity = new PaymentDueEntity()}
            };

            var actorMock = new Mock<IApprenticeshipPaymentsDueService>(MockBehavior.Strict);
            actorMock.Setup(a => a.HandlePayableEarning(earning, It.IsAny<CancellationToken>())).ReturnsAsync(paymentsDueEvents).Verifiable();

            var proxyFactoryMock = new Mock<IActorProxyFactory>(MockBehavior.Strict);
            proxyFactoryMock.Setup(f => f.CreateActorProxy<IApprenticeshipPaymentsDueService>(It.IsAny<Uri>(), It.IsAny<ActorId>(), null)).Returns(actorMock.Object).Verifiable();

            IHandleMessages<IPayableEarningEvent> handler = new PayableEarningEventHandler(apprenticeshipKeyServiceMock.Object, endpoint.Object, proxyFactoryMock.Object);

            // act
            await handler.Handle(earning, null); 

            // assert
            proxyFactoryMock.Verify();
            actorMock.Verify();
            endpoint.Verify();
            apprenticeshipKeyServiceMock.Verify();
        }
    }
}
