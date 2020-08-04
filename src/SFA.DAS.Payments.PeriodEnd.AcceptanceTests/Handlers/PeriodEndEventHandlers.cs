using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.PeriodEnd.AcceptanceTests.Handlers
{
    public class PeriodEndStartedEventHandler: IHandleMessages<PeriodEndStartedEvent>
    {
        public static readonly List<PeriodEndStartedEvent> ReceivedEvents = new List<PeriodEndStartedEvent>();

        public Task Handle(PeriodEndStartedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }

    public class PeriodEndRunningEventHandler : IHandleMessages<PeriodEndRunningEvent>
    {
        public static readonly List<PeriodEndRunningEvent> ReceivedEvents = new List<PeriodEndRunningEvent>();

        public Task Handle(PeriodEndRunningEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }

    public class PeriodEndStoppedEventHandler : IHandleMessages<PeriodEndStoppedEvent>
    {
        public static readonly List<PeriodEndStoppedEvent> ReceivedEvents = new List<PeriodEndStoppedEvent>();

        public Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }


    public class PeriodEndRequestReportsEventHandlers: IHandleMessages<PeriodEndRequestReportsEvent>
    {
        public static readonly List<PeriodEndRequestReportsEvent> ReceivedEvents = new List<PeriodEndRequestReportsEvent>();

        public Task Handle(PeriodEndRequestReportsEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
    public class PeriodEndRequestValidateSubmissionWindow: IHandleMessages<PeriodEndRequestValidateSubmissionWindowEvent>
    {
        public static readonly List<PeriodEndRequestValidateSubmissionWindowEvent> ReceivedEvents = new List<PeriodEndRequestValidateSubmissionWindowEvent>();

        public Task Handle(PeriodEndRequestValidateSubmissionWindowEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received message: {message.ToJson()}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }
    }
}