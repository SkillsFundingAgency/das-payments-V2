using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using Earning = SFA.DAS.Payments.RequiredPayments.Domain.Entities.Earning;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class PayableEarningEventProcessorTest
    {
        private AutoMock mocker;
        private Mapper mapper;
        private IPayableEarningEventProcessor processor;
        private Mock<IDataCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IRequiredPaymentService> requiredPaymentsService;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            requiredPaymentsService = mocker.Mock<IRequiredPaymentService>();
            paymentHistoryCacheMock = mocker.Mock<IDataCache<PaymentHistoryEntity[]>>();
 
            processor = mocker.Create<PayableEarningEventProcessor>(
                new NamedParameter("apprenticeshipKey", "key"),
                new NamedParameter("mapper", mapper));
        }
        
        [TearDown]
        public void TearDown()
        {
            requiredPaymentsService.Verify();
            paymentHistoryCacheMock.Verify();
        }

        
        [Test]
        public async Task TestHandleNullEvent()
        {
            // arrange            
            // act
            // assert
            try
            {
                await processor.HandleEarningEvent(null, paymentHistoryCacheMock.Object, CancellationToken.None);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("earningEvent", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task TestNormalEvent()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);

            var earningEvent = new PayableEarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = period.Period,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            },
                            new EarningPeriod
                            {
                                Amount = 200,
                                Period = (byte) (period.Period + 1),
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                }
            };

            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 100,
                    EarningType = EarningType.Levy,
                },
            };

            var paymentHistoryEntities = new[] {new PaymentHistoryEntity {CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2), DeliveryPeriod = 2}};
            mocker.Mock<IPaymentKeyService>()
                .Setup(s => s.GeneratePaymentKey("9", 1, It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            requiredPaymentsService.Setup(p => p.GetRequiredPayments(It.IsAny<Earning>(), It.IsAny<List<Payment>>()))
                .Returns(requiredPayments)
                .Verifiable();
          
            // act           
            var actualRequiredPayment = await processor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            actualRequiredPayment.Should().HaveCount(1);
        }
    }
}
