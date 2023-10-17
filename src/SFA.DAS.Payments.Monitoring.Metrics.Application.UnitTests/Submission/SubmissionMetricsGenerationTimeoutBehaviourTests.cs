using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using NServiceBus.Pipeline;
using NServiceBus.Testing;
using NServiceBus.Unicast.Messages;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Messages.Commands;
using AutoFixture;
using Moq;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Infrastructure.Messaging;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionMetricsGenerationTimeoutBehaviourTests
    {
        private Fixture _fixture;
        private TestableIncomingLogicalMessageContext _context;
        private GenerateSubmissionSummary _message;
        private SubmissionMetricsGenerationTimeoutBehaviour _sut;
        private AutoMock moqer;
        private Mock<ITelemetry> _telemetry;

        [SetUp]
        public void Setup()
        {
            moqer = AutoMock.GetLoose();
            _fixture = new Fixture();
            _message = _fixture.Create<GenerateSubmissionSummary>();
            
            _context = new TestableIncomingLogicalMessageContext
            {
                Message = new LogicalMessage(new MessageMetadata(typeof(GenerateSubmissionSummary)), _message)
            };

            _telemetry = moqer.Mock<ITelemetry>();
            _telemetry.Setup(x => x.TrackEvent(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Dictionary<string, double>>()));

            _sut = new SubmissionMetricsGenerationTimeoutBehaviour(_telemetry.Object);
        }

        [Test]
        public async Task Handles_Submission_Metrics_Timeout_Exception()
        {
            Func<Task> next = () => throw new SubmissionMetricsTimeoutException("Timed out");

            await _sut.Invoke(_context, next);

            var expectedMessage = $"Submission metrics generation timed out for UKPRN {_message.Ukprn}";
            _telemetry.Verify(x => x.TrackEvent(expectedMessage, 
                It.IsAny<Dictionary<string, string>>(), 
                It.IsAny<Dictionary<string, double>>()), Times.Once);
            _telemetry.Verify(x => x.TrackEvent(expectedMessage, 
                It.Is<Dictionary<string, string>>(y => DictionaryContainsValues(y, _message)), 
                It.IsAny<Dictionary<string,double>>()), Times.Once);
        }

        private static bool DictionaryContainsValues(Dictionary<string, string> dictionary, GenerateSubmissionSummary message)
        {
            Assert.AreEqual(message.AcademicYear.ToString(), dictionary[TelemetryKeys.AcademicYear]);
            Assert.AreEqual(message.CollectionPeriod.ToString(), dictionary[TelemetryKeys.CollectionPeriod]);
            Assert.AreEqual(message.Ukprn.ToString(), dictionary[TelemetryKeys.Ukprn]);
            Assert.AreEqual(message.JobId.ToString(), dictionary[TelemetryKeys.JobId]);

            return true;
        }
    }
}
