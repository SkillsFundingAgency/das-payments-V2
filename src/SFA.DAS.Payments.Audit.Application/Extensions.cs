using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Common.Events;

namespace SFA.DAS.Payments.Audit.Application
{
    public static class Extensions
    {
        public static string ToDebug(this IPaymentsEvent paymentsEvent)
        {
            return $"Type: {paymentsEvent.GetType().Name}, Id: {paymentsEvent.EventId}, Event Time: {paymentsEvent.EventTime:G}, Job Id: {paymentsEvent.JobId}, Collection Period: {paymentsEvent.CollectionPeriod.AcademicYear}-{paymentsEvent.CollectionPeriod.Period}, Learner: {paymentsEvent.Learner.ReferenceNumber}";
        }

        public static string ToDebug(this SubmissionEvent submissionEvent)
        {
            return
                $"Type: {submissionEvent.GetType().Name}, Event Time: {submissionEvent.EventTime:G}, Job Id: {submissionEvent.JobId}, Collection Period: {submissionEvent.AcademicYear}-{submissionEvent.CollectionPeriod}";
        }
        
        public static void AddSqlParameter(this List<SqlParameter> sqlParameters, int index, object value)
        {
            sqlParameters.Add(new SqlParameter($@"p{index}_{sqlParameters.Count}", value));
        }

        public static void AddSqlParameter(this List<SqlParameter> sqlParameters, int index, int childIndex, object value)
        {
            sqlParameters.Add(new SqlParameter($@"p{index}_{childIndex}_{sqlParameters.Count}", value));
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

        public static bool IsUniqueKeyConstraintException(this Exception exception)
        {
            var sqlException = exception.GetException<Microsoft.Data.SqlClient.SqlException>();
            if (sqlException != null)
                return sqlException.Number == 2601 || sqlException.Number == 2627;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        public static bool IsDeadLockException(this Exception exception)
        {
            var sqlException = exception.GetException<Microsoft.Data.SqlClient.SqlException>();
            if (sqlException != null)
                return sqlException.Number == 1205;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && sqlEx.Number == 1205;
        }
    }
}