using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class RemovedLearnerAimIdentificationServiceTest
    {
        [Test]
        public async Task TestServiceReturnsRemovedAims()
        {
            // arrange
            var repositoryMock = new Mock<IPaymentHistoryRepository>(MockBehavior.Strict);
            var service = new RemovedLearnerAimIdentificationService(repositoryMock.Object);

            var expectedAims = new List<IdentifiedRemovedLearningAim>()
            {
                new IdentifiedRemovedLearningAim
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    Ukprn = 3,
                },
                new IdentifiedRemovedLearningAim
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    Ukprn = 3,
                }
            };

            repositoryMock.Setup(r => r.IdentifyRemovedLearnerAims(1, 2, 3, CancellationToken.None)).ReturnsAsync(expectedAims).Verifiable();

            // act
            var actualAims = await service.IdentifyRemovedLearnerAims(1, 2, 3, CancellationToken.None).ConfigureAwait(false);

            // assert
            actualAims.Should().BeSameAs(expectedAims);

            repositoryMock.Verify();
        }
    }
}
