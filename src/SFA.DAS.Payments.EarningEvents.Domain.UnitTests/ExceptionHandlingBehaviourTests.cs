﻿using NUnit.Framework;
using SFA.DAS.Payments.Core;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class ExceptionHandlingBehaviourTests
    { 
        [Test]
        public void Test_typename_extraction()
        {
            var messageType = "SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands.ProcessLearnerCommand, SFA.DAS.Payments.EarningEvents.Messages.Internal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;SFA.DAS.Payments.Messages.Common.Commands.IPaymentsCommand, SFA.DAS.Payments.Messages.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;SFA.DAS.Payments.Messages.Common.IJobMessage, SFA.DAS.Payments.Messages.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;SFA.DAS.Payments.Messages.Common.Commands.ICommand, SFA.DAS.Payments.Messages.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;SFA.DAS.Payments.Messages.Common.Commands.PaymentsCommand, SFA.DAS.Payments.Messages.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;SFA.DAS.Payments.Messages.Common.IPaymentsMessage, SFA.DAS.Payments.Messages.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var actual = TypeString.TryParseTypeName(messageType, out var name);
            Assert.IsTrue(actual);
            Assert.AreEqual("ProcessLearnerCommand", name);
        }
    }
}
