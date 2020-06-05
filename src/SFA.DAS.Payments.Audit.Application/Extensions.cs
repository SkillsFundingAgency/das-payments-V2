﻿using System;
using System.Collections.Generic;
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
        
        public static void AddSqlParameter(this List<SqlParameter> sqlParameters, int index, object value)
        {
            sqlParameters.Add(new SqlParameter($@"p{index}_{sqlParameters.Count}", value));
        }

        public static void AddSqlParameter(this List<SqlParameter> sqlParameters, int index, int childIndex, object value)
        {
            sqlParameters.Add(new SqlParameter($@"p{index}_{childIndex}_{sqlParameters.Count}", value));
        }

    }
}