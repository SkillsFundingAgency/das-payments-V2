using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Audit.Application
{
    public static class Extensions
    {
        public static string ToDebug(this IPaymentsEvent paymentsEvent)
        {
            return $"Type: {paymentsEvent.GetType().Name}, Id: {paymentsEvent.EventId}, Event Time: {paymentsEvent.EventTime:G}, Ukrpn: {paymentsEvent.Ukprn}, Job Id: {paymentsEvent.JobId}, Collection Period: {paymentsEvent.CollectionPeriod.AcademicYear}-{paymentsEvent.CollectionPeriod.Period}, Learner: {paymentsEvent.Learner.ReferenceNumber}";
        }
    }
}