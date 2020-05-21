using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System.Linq;
using SFA.DAS.Payments.EarningEvents.Application.Services;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class EarningsEventBuilderTests
    {
        ApprenticeshipContractTypeEarningsEventBuilder sut = new ApprenticeshipContractTypeEarningsEventBuilder(
            new ApprenticeshipContractTypeEarningsEventFactory(),
            new RedundancyEarningService(new RedundancyEarningEventFactory(new Mapper(new MapperConfiguration(c => { })))), 
            new Mapper(new MapperConfiguration(c => { })));

        [Test]
        public void Generate_No_Earnings_When_Fm36_Only_Has_Invalid_Contracts()
        {
            var cmd = new LearnerBuilder()
                .BuildLearnerCommand();

            // Arrange
            cmd.Learner.PriceEpisodes.First().PriceEpisodeValues.PriceEpisodeContractType = "none";

            // Act
            var earningsEvents = sut.Build(cmd);

            // Assert
            earningsEvents.Should().BeEmpty();
        }

        [Test]
        public void Generate_Valid_Earnings_When_Ignoring_Invalid_Contracts_In_Fm36()
        {
            var cmd = new LearnerBuilder()
                .WithMultipleDeliveries()
                .BuildLearnerCommand();

            // Arrange
            cmd.Learner.PriceEpisodes.First().PriceEpisodeValues.PriceEpisodeContractType = null;

            // Act
            var earningsEvents = sut.Build(cmd);

            // Assert
            var expectedEarnings = cmd.Learner.PriceEpisodes.Skip(1).Select(x => new ApprenticeshipContractType1EarningEvent
            {
                StartDate = x.PriceEpisodeValues.EpisodeStartDate.GetValueOrDefault()
            });
            
            earningsEvents.Should().BeEquivalentTo(expectedEarnings);
        }
    }
}