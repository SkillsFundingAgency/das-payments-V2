using System;
using System.Data;
using SFA.DAS.Payments.Audit.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public class ProviderPaymentDataTable : PeriodisedPaymentsEventModelDataTable<ProviderPaymentEventModel>
    {
        public ProviderPaymentDataTable()
        {
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("FundingSourceEventId", typeof(Guid)),
                new DataColumn("FundingSource"),
                new DataColumn("ApprenticeshipEmployerType", typeof(byte)),
                new DataColumn("ReportingAimFundingLineType", typeof(string)),
            });
        }

        protected override DataRow CreateDataRow(ProviderPaymentEventModel eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["FundingSourceEventId"] = eventModel.FundingSourceId;
            dataRow["FundingSource"] = (byte)eventModel.FundingSource;
            dataRow["ApprenticeshipEmployerType"] = eventModel.ApprenticeshipEmployerType;
            dataRow["ReportingAimFundingLineType"] = eventModel.ReportingAimFundingLineType;
            return dataRow;
        }

        public override string TableName => "Payments2.Payment";
    }
}