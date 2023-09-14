using System.Net;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Triggers;

namespace SFA.DAS.Payments.Audit.ArchiveService.UnitTests.Activities;

[TestFixture]
public class StartPeriodEndArchiveActivityTests
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
    public async Task WhenActivity_IsTriggered_ThenArchivingIsStarted()
    {
        var req = new HttpRequestMessage { Method = HttpMethod.Post };

        var response = await PeriodEndArchiveHttpTrigger.HttpStart(req, mockOrchestrationClient.Object,
            mockEntityClient.Object, logger.Object);
        var content = await response.Content.ReadAsStringAsync();

        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        content.Should().Be("Started orchestrator [PeriodEndArchiveOrchestrator] with ID [1234]\n\n\n\n");
    }
}