using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.ProviderAdjustments.Application;
using SFA.DAS.Payments.ProviderAdjustments.Application.Repositories;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.UnitTests
{
    [TestFixture]
    public class ProviderAdjustmentsProcessorTests
    {
        private Mock<IProviderAdjustmentRepository> providerAdjustmentRepositoryMock;
        private Mock<IPaymentLogger> loggerMock;
        private Mock<IProviderAdjustmentsCalculator> calculatorMock;
        private Mock<ITelemetry> telemetryMock;
        private ProviderAdjustmentsProcessor sut;

        private int academicYear;
        private int collectionPeriod;

        private List<ProviderAdjustment> previousProviderAdjustments;
        private List<ProviderAdjustment> currentProviderAdjustments;
        private List<ProviderAdjustment> payments;

        [SetUp]
        public void SetUp()
        {
            var fixture = new Fixture();

            academicYear = fixture.Create<int>();
            collectionPeriod = fixture.Create<int>();
            previousProviderAdjustments = fixture.Create<List<ProviderAdjustment>>();
            currentProviderAdjustments = fixture.Create<List<ProviderAdjustment>>();
            payments = fixture.Create<List<ProviderAdjustment>>();

            providerAdjustmentRepositoryMock = new Mock<IProviderAdjustmentRepository>();
            providerAdjustmentRepositoryMock
                .Setup(x => x.GetPreviousProviderAdjustments(academicYear))
                .ReturnsAsync(previousProviderAdjustments);
            providerAdjustmentRepositoryMock
                .Setup(x => x.GetCurrentProviderAdjustments(academicYear))
                .ReturnsAsync(currentProviderAdjustments);

            calculatorMock = new Mock<IProviderAdjustmentsCalculator>();
            calculatorMock
                .Setup(x => x.CalculateDelta(previousProviderAdjustments, currentProviderAdjustments, academicYear, collectionPeriod))
                .Returns(payments);

            loggerMock = new Mock<IPaymentLogger>();
            telemetryMock = new Mock<ITelemetry>();


            sut = new ProviderAdjustmentsProcessor(providerAdjustmentRepositoryMock.Object, loggerMock.Object, calculatorMock.Object, telemetryMock.Object);
        }

        [Test]
        public async Task WhenProcessingEasAtMonthEnd_AndProcessSucceeded_ThenTelemetrySent()
        {
            //Act
            await sut.ProcessEasAtMonthEnd(academicYear, collectionPeriod);

            //Assert
            telemetryMock.Verify(x => x.TrackEvent(It.IsRegex($"Finished processing EAS.*{academicYear}.*{collectionPeriod}")));
        }

        [Test]
        public void WhenProcessingEasAtMonthEnd_AndExceptionThrown_ThenTelemetrySent()
        {
            //Arrange
            calculatorMock
                .Setup(x => x.CalculateDelta(previousProviderAdjustments, currentProviderAdjustments, academicYear, collectionPeriod))
                .Throws<KeyNotFoundException>();

            //Act
            Assert.ThrowsAsync<KeyNotFoundException>(() => sut.ProcessEasAtMonthEnd(academicYear, collectionPeriod));

            //Assert
            telemetryMock.Verify(x => x.TrackEvent(It.IsRegex($"Failed.*{academicYear}.*{collectionPeriod}")));
        }

    }
}
