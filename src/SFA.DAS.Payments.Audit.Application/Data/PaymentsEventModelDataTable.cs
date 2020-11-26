﻿using System;
using System.Collections.Generic;
using System.Data;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public abstract class PaymentsEventModelDataTable<T> : IPaymentsEventModelDataTable<T> where T : IPaymentsEventModel
    {
        protected DataTable DataTable { get; }

        protected PaymentsEventModelDataTable()
        {
            DataTable = new DataTable("PaymentsEventTable");

            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("EventId", typeof(Guid)),
                new DataColumn("CollectionPeriod"),
                new DataColumn("AcademicYear"),
                new DataColumn("LearnerReferenceNumber"),
                new DataColumn("LearnerUln"),
                new DataColumn("LearningAimReference"),
                new DataColumn("LearningAimProgrammeType"),
                new DataColumn("LearningAimStandardCode"),
                new DataColumn("LearningAimFrameworkCode"),
                new DataColumn("LearningAimPathwayCode"),
                new DataColumn("LearningAimFundingLineType"),
                new DataColumn("Ukprn"),
                new DataColumn("IlrSubmissionDateTime", typeof(DateTime)),
                new DataColumn("JobId"),
                new DataColumn("EventTime", typeof(DateTimeOffset)),
                new DataColumn("LearningStartDate",typeof(DateTime)) {AllowDBNull = true},
            });
        }

        protected virtual DataRow CreateDataRow(T eventModel)
        {
            var dataRow = DataTable.NewRow();
            dataRow["EventId"] = eventModel.EventId;
            dataRow["CollectionPeriod"] = eventModel.CollectionPeriod;
            dataRow["AcademicYear"] = eventModel.AcademicYear;
            dataRow["LearnerReferenceNumber"] = eventModel.LearnerReferenceNumber;
            dataRow["LearnerUln"] = eventModel.LearnerUln;
            dataRow["LearningAimReference"] = eventModel.LearningAimReference;
            dataRow["LearningAimProgrammeType"] = eventModel.LearningAimProgrammeType;
            dataRow["LearningAimStandardCode"] = eventModel.LearningAimStandardCode;
            dataRow["LearningAimFrameworkCode"] = eventModel.LearningAimFrameworkCode;
            dataRow["LearningAimPathwayCode"] = eventModel.LearningAimPathwayCode;
            dataRow["LearningAimFundingLineType"] = eventModel.LearningAimFundingLineType;
            dataRow["Ukprn"] = eventModel.Ukprn;
            dataRow["IlrSubmissionDateTime"] = eventModel.IlrSubmissionDateTime;
            dataRow["JobId"] = eventModel.JobId;
            dataRow["EventTime"] = eventModel.EventTime;

            if (!eventModel.LearningStartDate.HasValue || eventModel.LearningStartDate.Value == DateTime.MinValue)
            {
                dataRow["LearningStartDate"] = DBNull.Value;
            }
            else
            {
                dataRow["LearningStartDate"] = eventModel.LearningStartDate.Value;
            }

            return dataRow;
        }

        public abstract string TableName { get; }

        public virtual List<DataTable> GetDataTable(List<T> events)
        {
            DataTable.Rows.Clear();
            events.ForEach(ev => DataTable.Rows.Add(CreateDataRow(ev)));
            return new List<DataTable> { DataTable };
        }
    }
}