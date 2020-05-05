using System;
using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Builders;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Builders
{
    [TestFixture]
    public class FundingSourcePaymentEventBuilderTests
    {
        private long employerAccountId;
        private long jobId;

        private Mock<IMapper> mapper;
        private Mock<IPaymentProcessor> processor;


        private FundingSourcePaymentEventBuilder service;


        [SetUp]
        public void SetUp()
        {
            employerAccountId = 112;
            jobId = 114;
            mapper = new Mock<IMapper>();
            processor = new Mock<IPaymentProcessor>();
        }


        public void InitialTest()
        {
            var amount = new CalculatedRequiredLevyAmount
            {
                AccountId = employerAccountId,
                AmountDue = 3000,
                AgreementId = "Agreement Two"
            };

            //var results = service.BuildFundingSourcePaymentsForRequiredPayment(amount, employerAccountId, jobId);
           // results.Should().HaveCount(1);
        }


    }
}