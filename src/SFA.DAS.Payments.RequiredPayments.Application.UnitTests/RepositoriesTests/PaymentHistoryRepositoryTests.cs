using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.RepositoriesTests
{
    [TestFixture]
    public class PaymentHistoryRepositoryTests
    {
        private IPaymentsDataContext context;
        private PaymentHistoryRepository sut;

        [SetUp]
        public void Setup()
        {
            var dbName = Guid.NewGuid().ToString();

            var contextBuilder = new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            context = new PaymentsDataContext(contextBuilder);
            sut = new PaymentHistoryRepository(context);
        }

        [Test]
        public async Task GetEmployerCoInvestedPaymentHistoryTotal_RoundsAmountsDown()
        {
            var key = CreateTestApprenticeshipKey();
            var payment1 = CreateTestPayment();
            payment1.Amount = 100.99m;

            context.Payment.Add(payment1);
            context.SaveChanges();

            var actual = await sut.GetEmployerCoInvestedPaymentHistoryTotal(key);

            actual.Should().Be(100);
        }

        [Test]
        public async Task GetEmployerCoInvestedPaymentHistoryTotal_RoundsAllAmountsDown()
        {
            var key = CreateTestApprenticeshipKey();
            var payment1 = CreateTestPayment();
            payment1.Amount = 50.99m;

            var payment2 = CreateTestPayment();
            payment2.Amount = 50.99m;

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var actual = await sut.GetEmployerCoInvestedPaymentHistoryTotal(key);

            actual.Should().Be(100);
        }

        private PaymentModel CreateTestPayment()
        {
            return new PaymentModel
            {
                Ukprn = 101,
                LearningAimFrameworkCode = 102,
                LearningAimReference = "ref",
                LearnerReferenceNumber = "123456",
                LearningAimPathwayCode = 103,
                LearningAimProgrammeType = 104,
                LearningAimStandardCode = 105,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                ContractType = ContractType.Act2,
            };
        }

        [TestCase(ContractType.Act1)]
        [TestCase(ContractType.Act2)]
        public async Task GetLearnerPaymentHistoryReturnsAllPaymentsForLearner(ContractType contractType)
        {
            var payment1 = CreateTestPayment(50.99m, contractType);
            var payment2 = CreateTestPayment(-50.99m, contractType);

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetReadOnlyLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(2);
        }

        [Test]
        public async Task GetLearnerPaymentHistoryReturnsOnlyPaymentsForCurrentAcademicYear()
        {
            var contractType = ContractType.Act2;

            var payment1 = CreateTestPayment(50.99m, contractType);

            var payment2 = CreateTestPayment(-50.99m, contractType);
            payment2.CollectionPeriod = new CollectionPeriod { AcademicYear = 1920, Period = 2 };

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetReadOnlyLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(1);
        }

        [TestCase(ContractType.Act1)]
        [TestCase(ContractType.Act2)]
        public async Task GetLearnerPaymentHistoryReturnsAllPaymentsForPreviousCollectionPeriodsExcludingCurrentCollectionPeriod(ContractType contractType)
        {
            var payment1 = CreateTestPayment(50.99m, contractType);
            var payment2 = CreateTestPayment(-50.99m, contractType);
            payment2.CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 };

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var results = await sut.GetReadOnlyLearnerPaymentHistory(101, contractType, "123456", "ref", 102, 103, 104, 105, 2021, 2, CancellationToken.None);

            results.Count.Should().Be(1);
        }

        private PaymentModel CreateTestPayment(decimal amount, ContractType contractType)
        {
            return new PaymentModel
            {
                Ukprn = 101,
                LearnerReferenceNumber = "123456",
                LearningAimReference = "ref",
                LearningAimFrameworkCode = 102,
                LearningAimPathwayCode = 103,
                LearningAimProgrammeType = 104,
                LearningAimStandardCode = 105,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                Amount = amount,
                ContractType = contractType,
            };
        }

        private ApprenticeshipKey CreateTestApprenticeshipKey()
        {
            return new ApprenticeshipKey
            {
                Ukprn = 101,
                FrameworkCode = 102,
                LearnAimRef = "ref",
                LearnerReferenceNumber = "123456",
                PathwayCode = 103,
                ProgrammeType = 104,
                StandardCode = 105,
                ContractType = ContractType.Act2,
            };
        }
    }
}
