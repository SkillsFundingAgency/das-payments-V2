using System;
using System.Data.SqlClient;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Audit.Application
{
    public static class Extensions
    {
        public static string ToDebug(this IPaymentsEvent paymentsEvent)
        {
            return $"Type: {paymentsEvent.GetType().Name}, Id: {paymentsEvent.EventId}, Event Time: {paymentsEvent.EventTime:G}, Ukprn: {paymentsEvent.Ukprn}, Job Id: {paymentsEvent.JobId}, Collection Period: {paymentsEvent.CollectionPeriod.AcademicYear}-{paymentsEvent.CollectionPeriod.Period}, Learner: {paymentsEvent.Learner.ReferenceNumber}";
        }

        public static string ToDebug(this SubmissionEvent submissionEvent)
        {
            return
                $"Type: {submissionEvent.GetType().Name}, Event Time: {submissionEvent.EventTime:G}, Ukprn: {submissionEvent.Ukprn}, Job Id: {submissionEvent.JobId}, Collection Period: {submissionEvent.AcademicYear}-{submissionEvent.CollectionPeriod}";
        }

        public static bool IsUniqueKeyConstraint(this SqlException sqlException)
        {
            return sqlException != null && (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        public static T GetException<T>(this Exception e) where T : Exception
        {
            var innerEx = e;
            while (innerEx != null && !(innerEx is T))
            {
                innerEx = innerEx.InnerException;
            }

            return innerEx as T;
        }
        public static bool IsDeadLock(this SqlException sqlException)
        {
            return sqlException != null && sqlException.Number == 1205;
        }
    }
}