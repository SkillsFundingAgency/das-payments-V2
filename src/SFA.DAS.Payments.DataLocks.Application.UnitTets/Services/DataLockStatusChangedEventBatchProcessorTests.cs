using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Batch;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    class DataLockStatusChangedEventBatchProcessorTests
    {
        private DataLockStatusChangedEventBatchProcessor _processor;
        private Mock<IBatchedDataCache<DataLockStatusChanged>> _cache;
        private Mock<IBulkWriter<LegacyDataLockEvent>> _datalockEventWriter;
        private Mock<IApprenticeshipRepository> _apprenticeshipRepository;

        [SetUp]
        public void SetUp()
        {
            _cache = new Mock<IBatchedDataCache<DataLockStatusChanged>>();
            _datalockEventWriter = new Mock<IBulkWriter<LegacyDataLockEvent>>();
            _apprenticeshipRepository = new Mock<IApprenticeshipRepository>();
            _processor = new DataLockStatusChangedEventBatchProcessor(
                _cache.Object, 
                Mock.Of<IPaymentLogger>(),
                _datalockEventWriter.Object,
                Mock.Of<IBulkWriter<LegacyDataLockEventCommitmentVersion>>(),
                Mock.Of<IBulkWriter<LegacyDataLockEventError>>(),
                Mock.Of<IBulkWriter<LegacyDataLockEventPeriod>>(),
                _apprenticeshipRepository.Object);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("short", "short")]
        [TestCase("short-but-longer-than-9", "-longer-than-9" )]
        [TestCase("10003161/ILR-10003161-1819-20180906-161700-01-Tight.xml", "ILR-10003161-1819-20180906-161700-01-Tight.xml")]
        [TestCase("10003161/ILR-10003161-1819-20180906-161700-01-Tight-12345.xml", "ILR-10003161-1819-20180906-161700-01-Tight-12345.x")]
        public void IlrFileName_DoesNotExceed50Chars(string test, string expected)
        {
            var actual = DataLockStatusChangedEventBatchProcessor.TrimUkprnFromIlrFileNameLimitToValidLength(test);

            actual.Should().Be(expected);
        }

        [Test]
        public async Task WhenThereIsNoApprenticeshipIdThenNoDataLockEventIsCreated()
        {
            var batchSize = 100;
            var earningPeriod = new EarningPeriod { DataLockFailures = new EditableList<DataLockFailure>() };
            var expectedDataLock = new DataLockStatusChangedToFailed { TransactionTypesAndPeriods = new Dictionary<TransactionType, List<EarningPeriod>> { { TransactionType.Learning, new List<EarningPeriod> { earningPeriod } } } };

            _cache.Setup(x => x.GetPayments(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<DataLockStatusChanged> { expectedDataLock });
            _apprenticeshipRepository.Setup(x => x.Get(It.IsAny <List<long>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ApprenticeshipModel>());

            await _processor.Process(batchSize, CancellationToken.None);

            _datalockEventWriter.Verify(x => x.Write(It.IsAny<LegacyDataLockEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}