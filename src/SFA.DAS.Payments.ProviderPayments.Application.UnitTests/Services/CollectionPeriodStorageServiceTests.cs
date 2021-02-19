using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Services;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class CollectionPeriodStorageServiceTests
    {
        private CollectionPeriodStorageServiceFixture fixture;

        [SetUp]
        public void SetUp()
        {
            fixture = new CollectionPeriodStorageServiceFixture();
        }

        [Test]
        public async Task WhenStoringCollectionPeriod_AndCollectionPeriodAlreadyExists_ThenDoesNotSave()
        {
            fixture.WithExisting_CollectionPeriod_InDB();

            await fixture.Act();

            fixture.Assert_Db_OnlyContains_ExistingRecord();
        }

        [Test]
        public async Task WhenStoringCollectionPeriod_AndNoRefemrenceDataValidationDateFound_ThenLogsWarning()
        {
            await fixture.Act();

            fixture.Assert_ReferenceDataValidationDate_WarningLogged();
        }

        [Test]
        public async Task WhenStoringCollectionPeriod_ThenSavesCollectionPeriod()
        {
            await fixture.Act();

            fixture.Assert_CollectionPeriod_IsSaved();
        }
    }

    internal class CollectionPeriodStorageServiceFixture
    {
        private readonly short academicYear;
        private readonly byte period;
        private readonly DateTime completionDate;
        private readonly CollectionPeriodModel existingCollectionPeriod;

        private readonly IProviderPaymentsDataContext context;
        private readonly Mock<IPaymentLogger> logger;

        private readonly ICollectionPeriodStorageService sut;

        internal CollectionPeriodStorageServiceFixture()
        {
            var random = new Random();
            var fixture = new Fixture();

            academicYear = (short)random.Next(2021, 2060);
            period = (byte)random.Next(1, 14);
            completionDate = fixture.Create<DateTime>();

            existingCollectionPeriod = fixture.Create<CollectionPeriodModel>();
            existingCollectionPeriod.AcademicYear = academicYear;
            existingCollectionPeriod.Period = period;

            logger = new Mock<IPaymentLogger>();
            context = new ProviderPaymentsDataContext(new DbContextOptionsBuilder<ProviderPaymentsDataContext>().UseInMemoryDatabase("test", new InMemoryDatabaseRoot()).Options);
            sut = new CollectionPeriodStorageService(context, logger.Object);
        }

        internal Task Act() => sut.StoreCollectionPeriod(academicYear, period, completionDate);

        internal CollectionPeriodStorageServiceFixture WithExisting_CollectionPeriod_InDB()
        {
            context.CollectionPeriod.Add(existingCollectionPeriod);
            context.SaveChanges();
            return this;
        }

        internal void Assert_Db_OnlyContains_ExistingRecord()
        {
            context.CollectionPeriod.Contains(existingCollectionPeriod).Should().BeTrue();
            context.CollectionPeriod.Count().Should().Be(1);
            logger.VerifyNoOtherCalls();
        }

        internal void Assert_ReferenceDataValidationDate_WarningLogged()
        {
            logger.Verify(x => x.LogWarning(It.IsRegex($"Failed.*academic year: {academicYear}.*period: {period}"), It.IsAny<object[]>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        internal void Assert_CollectionPeriod_IsSaved()
        {
            context.CollectionPeriod
                .Any(x => x.AcademicYear == academicYear && x.Period == period && x.CompletionDate == completionDate)
                .Should().BeTrue();
        }
    }
}