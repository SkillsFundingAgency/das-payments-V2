using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Model.Entities;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class SubmissionEventGeneratorServiceTest
    {
        private ISubmissionEventGeneratorService service;
        private Mock<ISubmittedPriceEpisodeRepository> submittedPriceEpisodeRepoMock;
        private readonly CancellationToken ct = CancellationToken.None;
        private List<LegacySubmissionEvent> writtenEvents;
        private Mock<IBulkWriter<LegacySubmissionEvent>> bulkWriterMock;
        private Mock<ISubmissionEventProcessor> submissionEventProcessorMock;
        private readonly Mock<IPaymentLogger> loggerMock = new Mock<IPaymentLogger>(MockBehavior.Loose);

        [SetUp]
        public void SetUp()
        {
            submissionEventProcessorMock = new Mock<ISubmissionEventProcessor>(MockBehavior.Strict);
            writtenEvents = new EditableList<LegacySubmissionEvent>();
            bulkWriterMock = new Mock<IBulkWriter<LegacySubmissionEvent>>(MockBehavior.Loose);
            bulkWriterMock.Setup(w => w.Write(It.IsAny<LegacySubmissionEvent>(), ct))
                .Callback<LegacySubmissionEvent, CancellationToken>((e, c) => writtenEvents.Add(e))
                .Returns(Task.CompletedTask);

            submittedPriceEpisodeRepoMock = new Mock<ISubmittedPriceEpisodeRepository>(MockBehavior.Strict);
            service = new SubmissionEventGeneratorService(submittedPriceEpisodeRepoMock.Object, bulkWriterMock.Object, loggerMock.Object, submissionEventProcessorMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            submissionEventProcessorMock.Verify();
            submittedPriceEpisodeRepoMock.Verify();
        }

        [Test]
        public async Task TestEventsAreWrittenWhenReturnedByDomainService()
        {
            // arrange
            var generatedEvent = new LegacySubmissionEvent();

            submissionEventProcessorMock.Setup(p => p.ProcessSubmission(It.IsAny<SubmittedPriceEpisodeEntity>(), null)).Returns(generatedEvent).Verifiable();
            submittedPriceEpisodeRepoMock.Setup(r => r.GetLastSubmittedPriceEpisodes(1, "2", ct)).ReturnsAsync(new List<SubmittedPriceEpisodeEntity>()).Verifiable();

            var newEvent = new ApprenticeshipContractType1EarningEvent
            {
                Ukprn = 1,
                Learner = new Learner {ReferenceNumber = "2"},
                LearningAim = new LearningAim(),
                PriceEpisodes = new List<PriceEpisode>{new PriceEpisode()},
                CollectionPeriod = new CollectionPeriod()
            };

            // act
            await service.ProcessEarningEvent(newEvent, ct).ConfigureAwait(false);

            // assert
            writtenEvents.Should().NotBeEmpty();
            writtenEvents.Should().HaveCount(1);
            writtenEvents[0].Should().BeSameAs(generatedEvent);
        }
    }
}
