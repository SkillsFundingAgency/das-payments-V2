using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Orchestrators;
using SFA.DAS.Payments.Audit.ArchiveService.Triggers;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Audit.ArchiveService.UnitTests.Triggers
{
    [TestFixture]
    public class PeriodEndArchiveHttpTriggerTests
    {
        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetLoose();
            mockOrchestrationClient = mocker.Mock<IDurableOrchestrationClient>();
            mockEntityClient = mocker.Mock<IDurableEntityClient>();
            logger = mocker.Mock<IPaymentLogger>();
        }

        private Mock<IPaymentLogger> logger;
        private Mock<IDurableEntityClient> mockEntityClient;
        private AutoMock mocker;
        private Mock<IDurableOrchestrationClient> mockOrchestrationClient;

        [Test]
        public async Task WhenHttpTrigger_ReceivesPostRequest_ThenOrchestratorIsStarted()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpMessage() };

            SetupRunningInstances(null);
            SetupStartOrchestration("1234", new HttpResponseMessage(HttpStatusCode.Accepted));

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
            content.Should().Be("Started orchestrator [PeriodEndArchiveOrchestrator] with ID [1234]\n\n\n\n");
        }

        [Test]
        public void WhenHttpTrigger_ReceivesPostRequest_WithoutContent_ThenOrchestratorIsNotStarted()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = null };

            Func<Task> act = async () => await PeriodEndArchiveHttpTrigger.HttpStart(req,
                mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            act.Should().ThrowAsync<Exception>()
                .WithMessage("Error in PeriodEndArchiveHttpTrigger. Request content is null. Request: {req}");
        }

        [Test]
        public async Task WhenHttpTrigger_ReceivesPostRequest_AndInstancesAlreadyExist_ThenOrchestratorIsNotStarted()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpMessage() };

            var orchestrationResult = new OrchestrationStatusQueryResult
            {
                DurableOrchestrationState = new List<DurableOrchestrationStatus>
                    { new() { CreatedTime = DateTime.Now, Name = "Instance01" } }
            };

            SetupStartOrchestration("1234", new HttpResponseMessage(HttpStatusCode.Accepted));
            SetupRunningInstances(orchestrationResult);

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            content.Should().Be("An instance of PeriodEndArchiveOrchestrator is already running.");
        }


        [Test]
        public async Task WhenHttpTrigger_ReceivesPostRequest_AndInstanceFailsToReturn_ThenErrorIsReceived()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpMessage() };
            const string orchestratorName = nameof(PeriodEndArchiveOrchestrator);


            SetupRunningInstances(null);
            SetupStartOrchestration_FailToReturnInstance("1234", orchestratorName);

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            content.Should().Be($"An error occurred starting [{orchestratorName}], no instance id was returned.");
        }

        [Test]
        public async Task WhenHttpTrigger_ReceivesPostRequest_AndInstanceCreateCheckStatusFails_ThenErrorIsReceived()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpMessage() };
            const string orchestratorName = nameof(PeriodEndArchiveOrchestrator);

            SetupRunningInstances(null);
            SetupStartOrchestration_FailCheckStatus("1234", orchestratorName);

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            content.Should()
                .Be($"An error occurred getting the status of [{orchestratorName}] for instance Id [1234].");
        }

        public StringContent SetupHttpMessage()
        {
            var model = new RecordPeriodEndFcsHandOverCompleteJob { CollectionPeriod = 11, CollectionYear = 2223 };
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                "application/json");
        }

        public void SetupStartOrchestration(string runId, HttpResponseMessage message)
        {
            mockOrchestrationClient
                .Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(runId);

            mockOrchestrationClient
                .Setup(x => x.CreateCheckStatusResponse(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), false))
                .Returns(message);
        }

        public void SetupRunningInstances(OrchestrationStatusQueryResult? queryResult)
        {
            mockOrchestrationClient
                .Setup(x => x.ListInstancesAsync(It.IsAny<OrchestrationStatusQueryCondition>(), CancellationToken.None))
                .ReturnsAsync(queryResult);
        }

        public void SetupStartOrchestration_FailToReturnInstance(string runId, string orchestratorName)
        {
            mockOrchestrationClient.Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("");
        }

        public void SetupStartOrchestration_FailCheckStatus(string runId, string orchestratorName)
        {
            mockOrchestrationClient
                .Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(runId);

            mockOrchestrationClient
                .Setup(x => x.CreateCheckStatusResponse(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), false))
                .Returns((HttpResponseMessage)null);
        }
    }
}