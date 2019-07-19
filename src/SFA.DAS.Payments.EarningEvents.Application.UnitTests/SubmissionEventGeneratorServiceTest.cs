using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Batch;
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
            service = new SubmissionEventGeneratorService();
        }

        [Test]
        public async Task TestEventsAreGeneratedOnNewSubmission()
        {
            // arrange
            var generatedEvent = new LegacySubmissionEvent();
            submissionEventProcessorMock.Setup(p => p.ProcessSubmission(It.IsAny<SubmittedPriceEpisodeEntity>(), null)).Returns(generatedEvent);
            submittedPriceEpisodeRepoMock.Setup(r => r.GetLastSubmittedPriceEpisodes(1, "2", ct)).ReturnsAsync(new List<SubmittedPriceEpisodeEntity>());
            var newEvent = new ApprenticeshipContractType1EarningEvent
            {
                Ukprn = 1,
                Learner = new Learner {ReferenceNumber = "2"}
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
