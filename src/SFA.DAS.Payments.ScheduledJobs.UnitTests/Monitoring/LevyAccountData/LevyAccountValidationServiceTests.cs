using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.LevyAccountData
{
    [TestFixture]
    public class LevyAccountValidationServiceTests
    {
        private readonly LevyAccountBuilder levyAccountBuilder = new LevyAccountBuilder();

        private LevyAccountValidationService sut;
        private AutoMock mocker;
        private Mock<ITelemetry> telemetry;
        private Mock<IDasLevyAccountApiWrapper> accountApiWrapper;
        private PaymentsDataContext paymentsDataContext;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IPaymentLogger>();

            accountApiWrapper = mocker.Mock<IDasLevyAccountApiWrapper>();

            telemetry = mocker.Mock<ITelemetry>();

            var levyAccountsValidator = new LevyAccountValidator(telemetry.Object);
            var combinedLevyAccountsValidator = new CombinedLevyAccountValidator(telemetry.Object, levyAccountsValidator);

            var contextBuilder = new DbContextOptionsBuilder<PaymentsDataContext>()
                                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                                 .Options;
            paymentsDataContext = new PaymentsDataContext(contextBuilder);
            paymentsDataContext.LevyAccount.AddRange(levyAccountBuilder.Build(1));
            paymentsDataContext.SaveChanges();

            sut = mocker.Create<LevyAccountValidationService>(
                new NamedParameter("validator", combinedLevyAccountsValidator),
                new NamedParameter("paymentsDataContext", paymentsDataContext));
        }

        [TearDown]
        public void Cleanup()
        {
            paymentsDataContext.Dispose();
            mocker.Dispose();
        }

        [Test]
        public void Validate_Should_RaiseInvalidDataValidationEventWhenDasLevyAccountApiWrapperReturnsNullOrEmptyList()
        {
            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync((List<LevyAccountModel>)null);

            Func<Task> act = async () => await sut.Validate();

            act.Should().NotThrow();

            telemetry.Verify(x => x.TrackEvent(It.Is<string>(s => s == "EmployerAccountReferenceData.Comparison.InvalidData"), It.IsAny<Dictionary<string, string>>(), null), Times.Once);
            telemetry.Verify(x => x.TrackEvent(It.IsAny<string>(), It.IsAny<Dictionary<string, double>>()), Times.Never);
        }

        [Test]
        public async Task Validate_Should_RaiseInvalidDataValidationEventWhenGetPaymentsLevyAccountDetailsReturnsNullOrEmptyList()
        {
            paymentsDataContext.RemoveRange(paymentsDataContext.LevyAccount.ToList());
            await paymentsDataContext.SaveChangesAsync();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccountBuilder.Build(1).ToList());

            Func<Task> act = async () => await sut.Validate();

            act.Should().NotThrow();

            telemetry.Verify(x => x.TrackEvent(It.Is<string>(s => s == "EmployerAccountReferenceData.Comparison.InvalidData"), It.IsAny<Dictionary<string, string>>(), null), Times.Once);
            telemetry.Verify(x => x.TrackEvent(It.IsAny<string>(), It.IsAny<Dictionary<string, double>>()), Times.Never);
        }

        [Test]
        public async Task Validate_Should_RunValidationRulesOnCombinedDataAndShouldRaiseCoreEventsAndShouldNotRaiseEmployerSpecificEvents()
        {
            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccountBuilder.Build(1).ToList());

            await sut.Validate();

            VerifyCombinedTelemetryEvent("das-LevyAccountCount");
            VerifyCombinedTelemetryEvent("das-IsLevyPayerCount");
            VerifyCombinedTelemetryEvent("das-TransferAllowanceTotal");
            VerifyCombinedTelemetryEvent("das-LevyAccountBalanceTotal");

            telemetry.Verify(x => x.TrackEvent(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), null), Times.Never);        
        }

        [Test]
        public async Task Validate_Should_RaiseDasLevyAccountCountMismatchValidationEvent()
        {
            paymentsDataContext.RemoveRange(paymentsDataContext.LevyAccount.ToList());
            await paymentsDataContext.SaveChangesAsync();

            await paymentsDataContext.LevyAccount.AddRangeAsync(levyAccountBuilder.Build(2).ToList());
            await paymentsDataContext.SaveChangesAsync();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccountBuilder.Build(1).ToList());

            await sut.Validate();

            VerifyCombinedTelemetryEvent("das-LevyAccountCount");
            VerifyCombinedTelemetryEvent("das-IsLevyPayerCount");
            VerifyCombinedTelemetryEvent("das-TransferAllowanceTotal");
            VerifyCombinedTelemetryEvent("das-LevyAccountBalanceTotal");


            VerifyIndividualTelemetryEvent("Das-MissingLevyAccount");
            VerifyIndividualTelemetryEvent("IsLevyPayerMismatch");
            VerifyIndividualTelemetryEvent("TransferAllowanceMismatch");
            VerifyIndividualTelemetryEvent("BalanceMismatch");
            
            VerifyIndividualTelemetryEvent("LevyAccountId", Times.Exactly(4));
        }

        [Test]
        public async Task Validate_ShouldRaisePaymentsLevyAccountCountMismatchValidationEvent()
        {
            var levyAccounts = levyAccountBuilder.Build(2).ToList();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccounts);

            await sut.Validate();

            VerifyCombinedTelemetryEvent("das-LevyAccountCount");
            VerifyCombinedTelemetryEvent("das-TransferAllowanceTotal");
            VerifyCombinedTelemetryEvent("das-LevyAccountBalanceTotal");

            VerifyIndividualTelemetryEvent("Payments-MissingLevyAccount");
            VerifyIndividualTelemetryEvent("TransferAllowanceMismatch");
            VerifyIndividualTelemetryEvent("BalanceMismatch");
        }

        [Test]
        public async Task Validate_Should_RaiseBalanceMismatchValidationEvent()
        {
            var levyAccounts = levyAccountBuilder.SetBalance(200).Build(1).ToList();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccounts);

            await sut.Validate();

            VerifyIndividualTelemetryEvent("BalanceMismatch");
            VerifyCombinedTelemetryEvent("das-LevyAccountBalanceTotal");
        }

        [Test]
        public async Task Validate_Should_RaiseTransferAllowanceMismatchValidationEvent()
        {
            var levyAccounts = levyAccountBuilder.SetTransferAllowance(200).Build(1).ToList();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccounts);

            await sut.Validate();

            VerifyIndividualTelemetryEvent("TransferAllowanceMismatch");
            VerifyCombinedTelemetryEvent("das-TransferAllowanceTotal");
        }

        [Test]
        public async Task Validate_Should_RaiseIsLevyPayerMismatchValidationEvent()
        {
            var levyAccounts = levyAccountBuilder.SetIsLevyPayer(false).Build(1).ToList();

            accountApiWrapper.Setup(x => x.GetDasLevyAccountDetails())
                             .ReturnsAsync(levyAccounts);

            await sut.Validate();

            VerifyIndividualTelemetryEvent("IsLevyPayerMismatch");
            VerifyCombinedTelemetryEvent("das-IsLevyPayerCount");
        }

        private void VerifyIndividualTelemetryEvent(string eventName, Times times = new Times())
        {
            telemetry.Verify(x =>
                                 x.TrackEvent(It.IsAny<string>(),
                                              It.Is<Dictionary<string, string>>(d => d.ContainsKey(eventName)),
                                              null),
                             times == default ? Times.Once() : times);
        }

        private void VerifyCombinedTelemetryEvent(string eventName)
        {
            telemetry.Verify(x =>
                                 x.TrackEvent(It.IsAny<string>(),
                                              It.Is<Dictionary<string, double>>(d => d.ContainsKey(eventName))),
                             Times.Once);
        }
    }
}
