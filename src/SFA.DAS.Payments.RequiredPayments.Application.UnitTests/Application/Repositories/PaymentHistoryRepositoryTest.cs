using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.RequiredPayments.Application.Data;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Repositories
{
    [TestFixture]
    public class PaymentHistoryRepositoryTest
    {
        private PaymentHistoryRepository _repository;
        private Mock<DbSet<PaymentHistoryEntity>> _paymentSetMock;
        private Mock<RequiredPaymentsDataContext> _dedsContextMock;
        private List<PaymentHistoryEntity> _paymentEntities;

        [SetUp]
        public void FixtureSetUp()
        {
            _paymentEntities = new List<PaymentHistoryEntity>();

            _paymentSetMock = new Mock<DbSet<PaymentHistoryEntity>>();

            _paymentSetMock.As<IAsyncEnumerable<PaymentHistoryEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestAsyncEnumerator<PaymentHistoryEntity>(_paymentEntities.GetEnumerator()));

            _paymentSetMock.As<IQueryable<PaymentHistoryEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<PaymentHistoryEntity>(_paymentEntities.AsQueryable().Provider));

            _paymentSetMock.As<IQueryable<PaymentHistoryEntity>>().Setup(m => m.Expression).Returns(_paymentEntities.AsQueryable().Expression);
            _paymentSetMock.As<IQueryable<PaymentHistoryEntity>>().Setup(m => m.ElementType).Returns(_paymentEntities.AsQueryable().ElementType);
            _paymentSetMock.As<IQueryable<PaymentHistoryEntity>>().Setup(m => m.GetEnumerator()).Returns(() => _paymentEntities.AsQueryable().GetEnumerator());

            _dedsContextMock = new Mock<RequiredPaymentsDataContext>(MockBehavior.Loose);
        }

        [TearDown]
        public void TearDown()
        {
            _dedsContextMock.Verify();
            _paymentSetMock.Verify();
        }

        [Test]
        public async Task TestGetPaymentHistory()
        {
            // arrange
            var apprenticeshipKey = "key";

            _paymentEntities.Add(new PaymentHistoryEntity {ExternalId = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});
            _paymentEntities.Add(new PaymentHistoryEntity {ExternalId = Guid.NewGuid(), ApprenticeshipKey = apprenticeshipKey});

            _dedsContextMock.Setup(c => c.PaymentHistory).Returns(_paymentSetMock.Object).Verifiable();
            _repository = new PaymentHistoryRepository(_dedsContextMock.Object);

            // act 
            var result = (await _repository.GetPaymentHistory(apprenticeshipKey).ConfigureAwait(false)).ToArray();

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_paymentEntities.First().ExternalId, result.First().ExternalId);
            Assert.AreEqual(_paymentEntities.Last().ExternalId, result.Last().ExternalId);
        }
    }
}