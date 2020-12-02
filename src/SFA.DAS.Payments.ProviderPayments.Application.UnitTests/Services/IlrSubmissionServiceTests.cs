using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class IlrSubmissionServiceTests
    {
        private AutoMock mocker;
        private HandleIlrSubmissionService ilrSubmissionService;

        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;
        private Mock<IDataCache<ReceivedProviderEarningsEvent>> ilrSubmittedEventCache;
        private Mock<IValidateIlrSubmission> validateIlrSubmission;


        private const long ukprn = 10000;
        private const long jobId = 10000;
        private List<PaymentModel> payments;
        private ReceivedProviderEarningsEvent receivedProviderEarningsEvent;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            payments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    Ukprn = ukprn,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    SfaContributionPercentage = 0.9m,
                    JobId = 1,
                    DeliveryPeriod = 7,
                    CollectionPeriod = new CollectionPeriod
                    {
                        AcademicYear = 1819,
                        Period = 8
                    },
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    ContractType = ContractType.Act1,
                    PriceEpisodeIdentifier = "P-1",
                    LearnerReferenceNumber = "100500",
                    EventId = Guid.NewGuid(),
                    LearningAimFundingLineType = "16-18",
                    LearningAimPathwayCode = 1,
                    LearningAimReference = "1",
                    LearningAimFrameworkCode = 1,
                    TransactionType = TransactionType.Learning,
                    LearnerUln = 1000,
                    LearningAimProgrammeType = 1,
                    LearningAimStandardCode = 1,
                    Amount = 2000.00m
                }
            };

            providerPaymentsRepository = mocker.Mock<IProviderPaymentsRepository>();
            providerPaymentsRepository
                .Setup(t => t.SavePayment(It.IsAny<PaymentModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            providerPaymentsRepository
                .Setup(o => o.GetMonthEndPayments(It.IsAny<CollectionPeriod>(), It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(payments)
                .Verifiable();

            providerPaymentsRepository
                .Setup(o => o.DeleteOldMonthEndPayment(It.IsAny<CollectionPeriod>(),
                    It.IsAny<long>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            receivedProviderEarningsEvent = new ReceivedProviderEarningsEvent
            {
                Ukprn = ukprn,
                JobId = jobId,
                IlrSubmissionDateTime = DateTime.MaxValue,
                CollectionPeriod = new CollectionPeriodBuilder().WithDate(new DateTime(2018, 2, 1)).Build(),
            };

            ilrSubmittedEventCache = mocker.Mock<IDataCache<ReceivedProviderEarningsEvent>>();
            ilrSubmittedEventCache
                .Setup(o => o.TryGet(ukprn.ToString(), default(CancellationToken)))
                .Returns(Task.FromResult(new ConditionalValue<ReceivedProviderEarningsEvent>(true, receivedProviderEarningsEvent)));

            ilrSubmittedEventCache
                .Setup(o => o.Clear(ukprn.ToString(), default(CancellationToken)))
                .Returns(Task.CompletedTask);

            ilrSubmittedEventCache
                .Setup(o => o.Add(ukprn.ToString(), It.IsAny<ReceivedProviderEarningsEvent>(), default(CancellationToken)))
                .Returns(Task.CompletedTask);

            validateIlrSubmission = mocker.Mock<IValidateIlrSubmission>();
            validateIlrSubmission
                .Setup(o => o.IsLatestIlrPayment(It.IsAny<IlrSubmissionValidationRequest>()))
                .Returns(true);


            ilrSubmissionService = mocker.Create<HandleIlrSubmissionService>();
        }
    }
}
