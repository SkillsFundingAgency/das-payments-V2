using System;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Messages.Core
{
    public static class Extensions
    {
        public static bool IsMessage(this Type type)
        {
            return (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false);
        }

        public static bool IsEvent(this Type type)
        {
            return IsMessage(type) && typeof(IPaymentsEvent).IsAssignableFrom(type);
        }
    }
}