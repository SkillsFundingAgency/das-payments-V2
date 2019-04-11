using AutoMapper;
using Castle.Components.DictionaryAdapter;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class FunctionalSkillEarningEventBuilderTest
    {
        [Test]
        public void TestBuild()
        {
            // arrange
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            var expectedResult = new FunctionalSkillEarningsEvent();
            mockMapper.Setup(m => m.Map<FunctionalSkillEarningsEvent>(It.IsAny<IntermediateLearningAim>())).Returns(expectedResult).Verifiable();
            var builder = new FunctionalSkillEarningEventBuilder(mockMapper.Object);
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                        new LearningDelivery {AimSeqNumber = 1, LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "ZPROG001"}},
                        new LearningDelivery
                        {
                            AimSeqNumber = 2, 
                            LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "M&E"},
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
                                    Period1 = 1,
                                    Period2 = 2
                                }
                            }
                        }
                    },
                    PriceEpisodes = new EditableList<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeValues = new PriceEpisodeValues {PriceEpisodeAimSeqNumber = 1},
                            PriceEpisodePeriodisedValues = new EditableList<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 10,
                                    Period2 = 20
                                }
                            }
                        }
                    }
                }
            };

            // act
            var events = builder.Build(learnerSubmission);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);
            events[0].Should().BeSameAs(expectedResult);

            mockMapper.Verify();
        }
    }
}
