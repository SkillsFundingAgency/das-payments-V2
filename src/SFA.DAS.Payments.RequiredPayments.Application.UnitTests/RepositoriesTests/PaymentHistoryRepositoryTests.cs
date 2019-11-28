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
    public class TestPaymentsContext : PaymentsDataContext
    {
        public TestPaymentsContext(DbContextOptions options) : base(options)
        {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }
    }

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
            context = new TestPaymentsContext(contextBuilder);
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
            context.SaveChanges();

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
