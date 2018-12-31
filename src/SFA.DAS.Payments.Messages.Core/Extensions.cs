using System;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Messages.Core
{
    public static class Extensions
    {
        public static bool IsMessage(this Type type)
        {
            return (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false);
        }

        public static bool IsEvent<TDomainEvent>(this Type type) where TDomainEvent: IPaymentsEvent
        {
            return IsMessage(type) && typeof(TDomainEvent).IsAssignableFrom(type);
        }

        public static bool IsCommand<TPaymentsCommand>(this Type type) where TPaymentsCommand : IPaymentsCommand
        {
            return IsMessage(type) && typeof(TPaymentsCommand).IsAssignableFrom(type);
        }
    }
}