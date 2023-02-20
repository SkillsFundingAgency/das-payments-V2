using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
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

            actual.Should().Be(101);
        }

        [TestCase(TransactionType.Completion)]
        [TestCase(TransactionType.Balancing)]
        [TestCase(TransactionType.First16To18EmployerIncentive)]
        [TestCase(TransactionType.First16To18ProviderIncentive)]
        [TestCase(TransactionType.Second16To18EmployerIncentive)]
        [TestCase(TransactionType.Second16To18ProviderIncentive)]
        [TestCase(TransactionType.OnProgramme16To18FrameworkUplift)]
        [TestCase(TransactionType.Completion16To18FrameworkUplift)]
        [TestCase(TransactionType.Balancing16To18FrameworkUplift)]
        [TestCase(TransactionType.FirstDisadvantagePayment)]
        [TestCase(TransactionType.SecondDisadvantagePayment)]
        [TestCase(TransactionType.OnProgrammeMathsAndEnglish)]
        [TestCase(TransactionType.BalancingMathsAndEnglish)]
        [TestCase(TransactionType.LearningSupport)]
        [TestCase(TransactionType.CareLeaverApprenticePayment)]
        public async Task GetEmployerCoInvestedPaymentHistoryTotal_Only_includes_Learning_Payments(TransactionType transactionType)
        {
            var key = CreateTestApprenticeshipKey();
            var payment1 = CreateTestPayment();
            payment1.Amount = 50.99m;

            var payment2 = CreateTestPayment();
            payment2.TransactionType = transactionType;
            payment2.Amount = 50.99m;

            context.Payment.Add(payment1);
            context.Payment.Add(payment2);
            await context.SaveChangesAsync();

            var actual = await sut.GetEmployerCoInvestedPaymentHistoryTotal(key);

            actual.Should().Be(50);
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
                TransactionType = TransactionType.Learning,
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
