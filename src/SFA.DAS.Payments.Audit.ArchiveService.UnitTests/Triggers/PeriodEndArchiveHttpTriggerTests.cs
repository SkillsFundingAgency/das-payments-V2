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
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
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
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpPostMessage() };

            SetupRunningInstances(null);
            SetupStartOrchestration("1234", new HttpResponseMessage(HttpStatusCode.Accepted));
            SetupMockRunInformation();

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

            SetupMockRunInformation();

            Func<Task> act = async () => await PeriodEndArchiveHttpTrigger.HttpStart(req,
                mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            act.Should().ThrowAsync<Exception>()
                .WithMessage("Error in PeriodEndArchiveHttpTrigger. Request content is null. Request: {req}");
        }

        [Test]
        public async Task WhenHttpTrigger_ReceivesPostRequest_AndInstancesAlreadyExist_ThenOrchestratorIsNotStarted()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpPostMessage() };

            var orchestrationResult = new OrchestrationStatusQueryResult
            {
                DurableOrchestrationState = new List<DurableOrchestrationStatus>
                    { new() { CreatedTime = DateTime.Now, Name = "Instance01" } }
            };

            SetupStartOrchestration("1234", new HttpResponseMessage(HttpStatusCode.Accepted));
            SetupRunningInstances(orchestrationResult);
            SetupMockRunInformation();

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
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpPostMessage() };
            const string orchestratorName = nameof(PeriodEndArchiveOrchestrator);


            SetupRunningInstances(null);
            SetupStartOrchestration_FailToReturnInstance();
            SetupMockRunInformation();

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
            var req = new HttpRequestMessage { Method = HttpMethod.Post, Content = SetupHttpPostMessage() };
            const string orchestratorName = nameof(PeriodEndArchiveOrchestrator);

            SetupRunningInstances(null);
            SetupStartOrchestration_FailCheckStatus("1234");
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            content.Should()
                .Be($"An error occurred getting the status of [{orchestratorName}] for instance Id [1234].");
        }

        [Test]
        public async Task WhenHttpTrigger_ReceivesGetRequest_AndJobId_DoesNotMatch_ShouldReturn_QueuedStatus()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = SetupHttpGetRequest("2345") };

            SetupRunningInstances(null);
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should()
                .Be("{\"JobId\":\"2345\",\"Status\":\"Queued\"}");
        }

        [Test]
        public async Task WhenHttpTrigger_ReceivesGetRequest_AndJobId_DoesMatch_ShouldReturn_CurrentStatus()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = SetupHttpGetRequest("1234") };

            SetupRunningInstances(null);
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should()
                .Be("{\"JobId\":\"1234\",\"Status\":\"Success\"}");
        }

        [Test]
        public async Task
            WhenHttpTrigger_ReceivesGetRequest_AndJobIdArgument_HasNotBeenPassed_ShouldThrowException()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = SetupHttpGetRequest(null) };

            SetupRunningInstances(null);
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            content.Should()
                .Be(
                    "Error in PeriodEndArchiveHttpTrigger. Invalid jobId. Request: Method: GET, RequestUri: 'http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator', Version: 1.1, Content: <null>, Headers:\r\n{\r\n}");
        }

        [Test]
        public async Task
            WhenHttpTrigger_ReceivesGetRequest_AndJobIdValue_HasNotBeenPassed_ShouldThrowException()
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator?jobId=")
            };

            SetupRunningInstances(null);
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            content.Should()
                .Be(
                    "Error in PeriodEndArchiveHttpTrigger. Invalid jobId. Request: Method: GET, RequestUri: 'http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator?jobId=', Version: 1.1, Content: <null>, Headers:\r\n{\r\n}");
        }


        [Test]
        public async Task
            WhenHttpTrigger_ReceivesGetRequest_AndJobIdValue_IsNotValidLong_ShouldThrowException()
        {
            var req = new HttpRequestMessage { Method = HttpMethod.Get, RequestUri = SetupHttpGetRequest("abcd") };

            SetupRunningInstances(null);
            SetupMockRunInformation();

            var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
                mockEntityClient.Object, logger.Object);
            var content = await response.Content.ReadAsStringAsync();

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            content.Should()
                .Be(
                    "Error in PeriodEndArchiveHttpTrigger. Invalid jobId. Request: Method: GET, RequestUri: 'http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator?jobId=abcd', Version: 1.1, Content: <null>, Headers:\r\n{\r\n}");
        }

        public StringContent SetupHttpPostMessage()
        {
            var model = new RecordPeriodEndFcsHandOverCompleteJob { CollectionPeriod = 11, CollectionYear = 2223 };
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                "application/json");
        }

        public static Uri SetupHttpGetRequest(string? jobId)
        {
            var uri = new Uri($"http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator?jobId={jobId}");
            if (string.IsNullOrEmpty(jobId))
            {
                uri = new Uri("http://localhost:7071/orchestrators/PeriodEndArchiveOrchestrator");
            }

            return uri;
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

        public void SetupStartOrchestration_FailToReturnInstance()
        {
            mockOrchestrationClient.Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("");
        }

        public void SetupStartOrchestration_FailCheckStatus(string runId)
        {
            mockOrchestrationClient
                .Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(runId);

            mockOrchestrationClient
                .Setup(x => x.CreateCheckStatusResponse(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), false))
                .Returns((HttpResponseMessage)null);
        }

        public void SetupMockRunInformation(string jobId = "1234")
        {
            mockEntityClient.Setup(x => x.ReadEntityStateAsync<RunInformation>(It.IsAny<EntityId>(), null, null))
                .ReturnsAsync(() => new EntityStateResponse<RunInformation>
                {
                    EntityExists = true, EntityState = new RunInformation { JobId = jobId, Status = "Success" }
                });
        }
    }
}