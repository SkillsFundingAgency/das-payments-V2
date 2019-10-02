using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain.Services;
using SFA.DAS.Payments.ProviderPayments.Model.V1;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services.PaymentExportServiceTests
{
    [TestFixture]
    public class PerformMonthEndTriggerTests
    {
        private AutoMock mocker;
        private PaymentExportService sut;
        private Mock<ILegacyPaymentsRepository> legacyPaymentsRepository;
        private Mock<IPaymentExportProgressCache> paymentExportProgressCache;
        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;
        private Mock<IPaymentMapper> paymentMapper;
        private CollectionPeriod testCollectionPeriod;
        private List<PaymentModelWithRequiredPaymentId> testPayments;
        private List<LegacyPaymentModel> testLegacyPayments;
        private List<LegacyRequiredPaymentModel> testLegacyRequiredPayments;
        private List<LegacyEarningModel> testLegacyEarnings;

        [SetUp]
        public void Setup()
        {
            mocker = AutoMock.GetLoose();
            legacyPaymentsRepository = mocker.Mock<ILegacyPaymentsRepository>();
            paymentExportProgressCache = mocker.Mock<IPaymentExportProgressCache>();
            providerPaymentsRepository = mocker.Mock<IProviderPaymentsRepository>();
            paymentMapper = mocker.Mock<IPaymentMapper>();

            sut = mocker.Create<PaymentExportService>();

            testCollectionPeriod = new CollectionPeriod
            {
                AcademicYear = (short) new Random().Next(short.MaxValue),
                Period = (byte) new Random().Next(byte.MaxValue),
            };

            testPayments = new List<PaymentModelWithRequiredPaymentId>();

            testLegacyPayments = new List<LegacyPaymentModel>();

            testLegacyRequiredPayments = new List<LegacyRequiredPaymentModel>();

            testLegacyEarnings = new List<LegacyEarningModel>();
        }

        [Test]
        public async Task CallsLegacyPaymentsRepository()
        {
            await sut.PerformMonthEndTrigger(testCollectionPeriod);

            legacyPaymentsRepository.Verify(x => x.WriteMonthEndTrigger(testCollectionPeriod));
        }

        [Test]
        public async Task UsesTheCorrectPage()
        {
            paymentExportProgressCache.Setup(x => x.GetPage(testCollectionPeriod.AcademicYear, testCollectionPeriod.Period))
                .ReturnsAsync(123);

            providerPaymentsRepository.Setup(x => x.GetMonthEndPayments(testCollectionPeriod, 10000, 123))
                .Returns(testPayments);

            await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);
        }

        [Test]
        public async Task StopsWhenThereAreZeroPayments()
        {
            providerPaymentsRepository.Setup(x => x.GetMonthEndPayments(testCollectionPeriod, 10000, It.IsAny<int>()))
                .Returns(testPayments);

            await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);
        }

        [Test]
        public async Task UsesTheCorrectlyMappedPayments()
        {
            testPayments.Add(new PaymentModelWithRequiredPaymentId());
            providerPaymentsRepository.SetupSequence(x => x.GetMonthEndPayments(testCollectionPeriod, 10000, It.IsAny<int>()))
                .Returns(testPayments)
                .Returns(new List<PaymentModelWithRequiredPaymentId>());

            paymentMapper.Setup(x => x.MapV2Payments(testPayments))
                .Returns((testLegacyPayments, testLegacyRequiredPayments, testLegacyEarnings))
                .Verifiable();

            await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);

            paymentMapper.Verify();
        }

        [Test]
        public async Task WritesPaymentInformation()
        {
            testPayments.Add(new PaymentModelWithRequiredPaymentId());
            providerPaymentsRepository.SetupSequence(x => x.GetMonthEndPayments(testCollectionPeriod, 10000, It.IsAny<int>()))
                .Returns(testPayments)
                .Returns(new List<PaymentModelWithRequiredPaymentId>());

            paymentMapper.Setup(x => x.MapV2Payments(testPayments))
                .Returns((testLegacyPayments, testLegacyRequiredPayments, testLegacyEarnings));

            legacyPaymentsRepository
                .Setup(x => x.WritePaymentInformation(testLegacyPayments, testLegacyRequiredPayments, testLegacyEarnings))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);

            legacyPaymentsRepository.Verify();
        }

        [Test]
        public async Task IncrementsThePage()
        {
            testPayments.Add(new PaymentModelWithRequiredPaymentId());
            providerPaymentsRepository.SetupSequence(x => x.GetMonthEndPayments(testCollectionPeriod, 10000, It.IsAny<int>()))
                .Returns(testPayments)
                .Returns(new List<PaymentModelWithRequiredPaymentId>());

            await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);

            paymentExportProgressCache.Verify(x => x.IncrementPage(testCollectionPeriod.AcademicYear, testCollectionPeriod.Period));
        }

        [Test]
        public void ThrowsAnExceptionWhenSomethingGoesWrong()
        {
            paymentExportProgressCache.Setup(x => x.GetPage(It.IsAny<short>(), It.IsAny<byte>()))
                .Throws<Exception>();

            Func<Task> action = async () => await sut.PerformExportPaymentsAndEarningsToV1(testCollectionPeriod);

            action.Should().Throw<Exception>();
        }
    }
}

