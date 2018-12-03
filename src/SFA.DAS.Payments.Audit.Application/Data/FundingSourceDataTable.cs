using System.Collections.Generic;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public class FundingSourceDataTable
    {
        protected DataTable DataTable { get; private set; }


        public FundingSourceDataTable()
        {
            DataTable = new DataTable("PaymentsEventTable");

            DataTable.Columns.AddRange(new []
            {
                new DataColumn("EventId"), 
                new DataColumn("RequiredPaymentEventId"),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("ContractType"),
                new DataColumn("TransactionType"),
                new DataColumn("FundingSourceType"),
                new DataColumn("Amount"),
                new DataColumn("CollectionPeriod"),
                new DataColumn("CollectionYear"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("LearnerReferenceNumber"),
                new DataColumn("LearnerUln"),
                new DataColumn("LearningAimReference"),
                new DataColumn("LearningAimProgrammeType"),
                new DataColumn("LearningAimStandardCode"),
                new DataColumn("LearningAimFrameworkCode"),
                new DataColumn("LearningAimPathwayCode"),
                new DataColumn("LearningAimFundingLineType"),
                new DataColumn("AgreementId"),
                new DataColumn("Ukprn"),
                new DataColumn("IlrSubmissionDateTime"),
                new DataColumn("JobId"),
                new DataColumn("EventTime")
            });


            //EventId UNIQUEIDENTIFIER NOT NULL,
            //    RequiredPaymentEventId UNIQUEIDENTIFIER NOT NULL, 
            //     NVARCHAR(50) NOT NULL,
            //      TINYINT NOT NULL,
            //     TINYINT NOT NULL,
            //     TINYINT NOT NULL,
            //     DECIMAL(15,5) NOT NULL,
            //     TINYINT NOT NULL,
            //     NVARCHAR(4) NOT NULL,
            //     TINYINT NOT NULL,
            //     NVARCHAR(50) NOT NULL,
            //      BIGINT NOT NULL,
            //     NVARCHAR(8) NOT NULL,
            //     INT NOT NULL ,
            //     INT NOT NULL,
            //     INT NOT NULL,
            //     INT NOT NULL,
            //      NVARCHAR(100) NOT NULL,
            //     NVARCHAR(255) NULL, 
            // BIGINT NOT NULL,
            //     DATETIME2 NOT NULL,
            //     BIGINT NOT NULL,
            //     DATETIME2 NOT NULL,

        }

        public DataTable GetBatch(List<FundingSourceEventModel> events)
        {
            DataTable.Rows.Clear();
            events.ForEach(ev =>  DataTable.Rows.Add(CreateDataRow(ev)));
            return DataTable;
        }

        protected DataRow CreateDataRow(FundingSourceEventModel eventModel)
        {
            var dataRow = DataTable.NewRow();
            dataRow["EventId"] = eventModel.EventId;
            dataRow["RequiredPaymentEventId"] = eventModel.RequiredPaymentEventId;
            dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier;
            dataRow["ContractType"] = (byte)eventModel.ContractType;
            dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            dataRow["FundingSourceType"] = (byte)eventModel.FundingSource;
            dataRow["Amount"] = eventModel.Amount;
            dataRow["CollectionPeriod"] = eventModel.CollectionPeriod;
            dataRow["CollectionYear"] = eventModel.CollectionYear;
            dataRow["DeliveryPeriod"] = eventModel.DeliveryPeriod;
            dataRow["LearnerReferenceNumber"] = eventModel.LearnerReferenceNumber;
            dataRow["LearnerUln"] = eventModel.LearnerUln;
            dataRow["LearningAimReference"] = eventModel.LearningAimReference;
            dataRow["LearningAimProgrammeType"] = eventModel.LearningAimProgrammeType;
            dataRow["LearningAimStandardCode"] = eventModel.LearningAimStandardCode;
            dataRow["LearningAimFrameworkCode"] = eventModel.LearningAimFrameworkCode;
            dataRow["LearningAimPathwayCode"] = eventModel.LearningAimPathwayCode;
            dataRow["LearningAimFundingLineType"] = eventModel.LearningAimFundingLineType;
            dataRow["AgreementId"] = eventModel.AgreementId;
            dataRow["Ukprn"] = eventModel.Ukprn;
            dataRow["IlrSubmissionDateTime"] = eventModel.IlrSubmissionDateTime;
            dataRow["JobId"] = eventModel.JobId;
            dataRow["EventTime"] = eventModel.EventTime;
            return dataRow;
        }
    }
}