using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{
    [TestFixture]
    public class RedundancyEarningEventFactoryTest
    {

      

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<EarningsEventProfile>(); });
            Mapper.AssertConfigurationIsValid();

        }



        
        [Test]
        [TestCase( typeof(ApprenticeshipContractType1EarningEvent), typeof(ApprenticeshipContractType1RedundancyEarningEvent))]
        [TestCase( typeof(ApprenticeshipContractType2EarningEvent), typeof(ApprenticeshipContractType2RedundancyEarningEvent))]
        public void CreateRedundancyContractType_CreatesCorrectContractTypeEvents(Type inputType, Type expectedType)
        {
            // arrange
            var factory = new RedundancyEarningEventFactory(Mapper.Instance);
            ApprenticeshipContractTypeEarningsEvent earningEvent;

            // act
                earningEvent = factory.CreateRedundancyContractType((ApprenticeshipContractTypeEarningsEvent)Activator.CreateInstance(inputType));

            // assert
            Assert.AreEqual(expectedType, earningEvent.GetType());
        }


        [Test]
        public void CreateRedundancyContractType_ForAct1ContractType_ShouldCreateANewEventId()
        {
            // arrange
            var factory = new RedundancyEarningEventFactory(Mapper.Instance);
            var earningEvent = new ApprenticeshipContractType1EarningEvent();
            earningEvent.EventId = Guid.NewGuid();

            // act
            var createdEvent = factory.CreateRedundancyContractType(earningEvent);
            // assert
            Assert.AreNotEqual(createdEvent.EventId, earningEvent.EventId );
        }
        [Test]
        public void CreateRedundancyContractType_ForAct2ContractType_ShouldCreateANewEventId()
        {
            // arrange
            var factory = new RedundancyEarningEventFactory(Mapper.Instance);
            var earningEvent = new ApprenticeshipContractType2EarningEvent();
            earningEvent.EventId = Guid.NewGuid();

            // act
            var createdEvent = factory.CreateRedundancyContractType(earningEvent);
            // assert
            Assert.AreNotEqual(createdEvent.EventId, earningEvent.EventId );
        }

        //[Test]
        //public void CreateRedundancyContractType_ForAct1RedundancyFunctionalSkillEarningsEvent_ShouldCreateANewEventId()
        //{
        //    // arrange
        //    var factory = new RedundancyEarningEventFactory(Mapper.Instance);
        //    var earningEvent = new Act1FunctionalSkillEarningsEvent();
        //    earningEvent.EventId = Guid.NewGuid();

        //    // act
        //    try
        //    {
        //        var createdEvent = factory.CreateRedundancyFunctionalSkillType(earningEvent);
        //        Assert.AreNotEqual(createdEvent.EventId, earningEvent.EventId);
        //    }
        //    catch (Exception e)
        //    {
              
        //        throw;
        //    }
        //    // assert
         
        //}

        //[Test]
        //public void CreateRedundancyContractType_ForAct2RedundancyFunctionalSkillEarningsEvent_ShouldCreateANewEventId()
        //{
        //    // arrange
        //    var factory = new RedundancyEarningEventFactory(Mapper.Instance);
        //    var earningEvent = new Act2RedundancyFunctionalSkillEarningsEvent();
        //    earningEvent.EventId = Guid.NewGuid();

        //    // act
        //    var createdEvent = factory.CreateRedundancyFunctionalSkillType(earningEvent);
        //    // assert
        //    Assert.AreNotEqual(createdEvent.EventId, earningEvent.EventId );
        //}

    }
}