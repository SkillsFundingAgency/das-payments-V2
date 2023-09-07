using System.Net;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Orchestrators;
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
        triggerHelper = mocker.Mock<ITriggerHelper>();
    }

    private Mock<IPaymentLogger> logger;
    private Mock<IDurableEntityClient> mockEntityClient;
    private AutoMock mocker;
    private Mock<IDurableOrchestrationClient> mockOrchestrationClient;
    private Mock<ITriggerHelper> triggerHelper;

    [Test]
    public async Task WhenHttpTrigger_ReceivesPostRequest_ThenOrchestratorIsStarted()
    {
        var req = new HttpRequestMessage { Method = HttpMethod.Post };

        triggerHelper.Setup(x => x.GetRunningInstances(nameof(PeriodEndArchiveHttpTrigger),
                nameof(PeriodEndArchiveOrchestrator), mockOrchestrationClient.Object, logger.Object))
            .ReturnsAsync(new OrchestrationStatusQueryResult());

        mockOrchestrationClient.Setup(x => x.StartNewAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("1234");
        mockOrchestrationClient
            .Setup(x => x.CreateCheckStatusResponse(It.IsAny<HttpRequestMessage>(), It.IsAny<string>(), false))
            .Returns(new HttpResponseMessage(HttpStatusCode.Accepted));
        var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
            mockEntityClient.Object, logger.Object);
        var content = await response.Content.ReadAsStringAsync();

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        content.Should().Be("Started orchestrator [PeriodEndArchiveOrchestrator] with ID [1234]\n\n\n\n");
    }
}