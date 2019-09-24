using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.Client.UnitTests
{
    [TestFixture]
    public class EarningsJobClientTests
    {
        private Autofac.Extras.Moq.AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            mocker.Mock<IEndpointInstanceFactory>()
                .Setup(factory => factory.GetEndpointInstance())
                .ReturnsAsync(mocker.Mock<IEndpointInstance>().Object);
        }
    }
}