using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMoqCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Application.Data;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Repositories
{
    [TestFixture]
    public class PaymentHistoryRepositoryTest
    {
        private AutoMoqer _moqer;

        private PaymentHistoryRepository _repository;
        private Mock<IRepositoryCache<IEnumerable<PaymentEntity>>> _cacheMock;
        private Mock<DbSet<PaymentEntity>> _paymentSetMock;
        private Mock<RequiredPaymentsDataContext> _dedsContextMock;
        private List<PaymentEntity> _paymentEntities;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Mapper.Initialize(cfg => AutoMapperConfigurationFactory.CreateMappingConfig());
        }

        [SetUp]
        public void FixtureSetUp()
        {
            _moqer = new AutoMoqer();
            _cacheMock = _moqer.GetMock<IRepositoryCache<IEnumerable<PaymentEntity>>>(MockBehavior.Strict);
            _dedsContextMock = _moqer.GetMock<RequiredPaymentsDataContext>();

            _paymentEntities = new List<PaymentEntity>();
            _paymentSetMock = new Mock<DbSet<PaymentEntity>>(MockBehavior.Loose);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Provider).Returns(_paymentEntities.AsQueryable().Provider);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.Expression).Returns(_paymentEntities.AsQueryable().Expression);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.ElementType).Returns(_paymentEntities.AsQueryable().ElementType);
            _paymentSetMock.As<IQueryable<PaymentEntity>>().Setup(m => m.GetEnumerator()).Returns(_paymentEntities.GetEnumerator());

            _paymentSetMock.As<IAsyncEnumerable<PaymentEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(_paymentEntities.ToAsyncEnumerable().GetEnumerator);
        }

        [TearDown]
        public void TearDown()
        {
            _cacheMock.Verify();
            _dedsContextMock.Verify();
            _paymentSetMock.Verify();
        }

        [Test]
        public async Task TestGetPaymentHistoryPopulatesCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _cacheMock.Setup(c => c.TryGet(apprenticeshipKey, default(CancellationToken)))
                .Returns(Task.FromResult(new ConditionalValue<IEnumerable<PaymentEntity>>(false, null)))
                .Verifiable();
            _cacheMock.Setup(c => c.Add(apprenticeshipKey, It.IsAny<IEnumerable<PaymentEntity>>(), default(CancellationToken))).Returns(Task.FromResult(false)).Verifiable();

            _repository = _moqer.Resolve<PaymentHistoryRepository>();

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey).ConfigureAwait(false)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }

        [Test]
        public async Task TestGetPaymentHistoryUsesCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _cacheMock.Setup(c => c.TryGet(apprenticeshipKey, default(CancellationToken)))
                .Returns(Task.FromResult(new ConditionalValue<IEnumerable<PaymentEntity>>(true, _paymentEntities)))
                .Verifiable();

            _repository = _moqer.Resolve<PaymentHistoryRepository>();

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey).ConfigureAwait(false)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }

        [Test]
        public async Task TestGetPaymentHistoryCanWorkWithoutCache()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentEntity {Id = Guid.NewGuid(), ApprenticeshipKey = "some other key"});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _repository = _moqer.Resolve<PaymentHistoryRepository>();

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey).ConfigureAwait(false)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities[0].Id, result.First().Id);
            Assert.AreEqual(_paymentEntities[1].Id, result.Last().Id);
        }
    }
}