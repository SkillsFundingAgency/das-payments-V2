using System.Net;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Triggers;

namespace SFA.DAS.Payments.Audit.ArchiveService.UnitTests.Triggers;

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
        var req = new HttpRequestMessage { Method = HttpMethod.Post };

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
    public async Task WhenHttpTrigger_ReceivesPostRequest_AndInstancesAlreadyExist_ThenOrchestratorIsNotStarted()
    {
        var req = new HttpRequestMessage { Method = HttpMethod.Post };

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

    public void SetupStartOrchestration(string runId, HttpResponseMessage message)
    {
        mockOrchestrationClient.Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(runId);
        mockOrchestrationClient
            .Setup(x => x.CreateCheckStatusResponse(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), false))
            .Returns(message);
    }

    public void SetupRunningInstances(OrchestrationStatusQueryResult? queryResult)
    {
        mockOrchestrationClient
            .Setup(x => x.ListInstancesAsync(It.IsAny<OrchestrationStatusQueryCondition>(), CancellationToken.None))
            .ReturnsAsync(queryResult
            );
    }
}