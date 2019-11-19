using System;
using System.Data;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public abstract class PeriodisedPaymentsEventModelDataTable<T>: PaymentsEventModelDataTable<T> where T: PeriodisedPaymentsEventModel
    {
        static readonly object DbNull = DBNull.Value;

        protected PeriodisedPaymentsEventModelDataTable()
        {
            DataTable.Columns.AddRange(new[]
            {
                new DataColumn("EarningEventId", typeof(Guid)),
                new DataColumn("PriceEpisodeIdentifier"),
                new DataColumn("ContractType"),
                new DataColumn("TransactionType"),
                new DataColumn("Amount"),
                new DataColumn("DeliveryPeriod"),
                new DataColumn("AgreementId"),
                new DataColumn("SfaContributionPercentage"),
                new DataColumn("AccountId"),
                new DataColumn("TransferSenderAccountId"),
                new DataColumn("EarningsStartDate",typeof(DateTime)),
                new DataColumn("EarningsPlannedEndDate",typeof(DateTime)) {AllowDBNull = true},
                new DataColumn("EarningsActualEndDate",typeof(DateTime)) {AllowDBNull = true},
                new DataColumn("EarningsCompletionStatus"),
                new DataColumn("EarningsCompletionAmount"),
                new DataColumn("EarningsInstalmentAmount"),
                new DataColumn("EarningsNumberOfInstalments"),
                new DataColumn("ApprenticeshipId",typeof(long)) {AllowDBNull = true},
                new DataColumn("ApprenticeshipPriceEpisodeId",typeof(long)) {AllowDBNull = true},
                new DataColumn("NonPaymentReason",typeof(long)) {AllowDBNull = true},
            });
        }

        protected override DataRow CreateDataRow(T eventModel)
        {
            var dataRow = base.CreateDataRow(eventModel);
            dataRow["EarningEventId"] = eventModel.EarningEventId;
            dataRow["PriceEpisodeIdentifier"] = eventModel.PriceEpisodeIdentifier ?? string.Empty;
            dataRow["ContractType"] = (byte)eventModel.ContractType;
            dataRow["TransactionType"] = (byte)eventModel.TransactionType;
            dataRow["Amount"] = eventModel.Amount;
            dataRow["DeliveryPeriod"] = eventModel.DeliveryPeriod;
            dataRow["AgreementId"] = eventModel.AgreementId;
            dataRow["SfaContributionPercentage"] = eventModel.SfaContributionPercentage;
            dataRow["AccountId"] = eventModel.AccountId;
            dataRow["TransferSenderAccountId"] = eventModel.TransferSenderAccountId;
            dataRow["EarningsStartDate"] = eventModel.StartDate == DateTime.MinValue ? DateTime.Today:eventModel.StartDate;
            dataRow["EarningsCompletionStatus"] = eventModel.CompletionStatus;
            dataRow["EarningsCompletionAmount"] = eventModel.CompletionAmount;
            dataRow["EarningsInstalmentAmount"] = eventModel.InstalmentAmount;
            dataRow["EarningsNumberOfInstalments"] = eventModel.NumberOfInstalments;

            if (!eventModel.PlannedEndDate.HasValue || eventModel.PlannedEndDate == DateTime.MinValue)
            {
                dataRow["EarningsPlannedEndDate"] = DBNull.Value;
            }
            else
            {
                dataRow["EarningsPlannedEndDate"] = eventModel.PlannedEndDate;
            }

            if (!eventModel.ActualEndDate.HasValue || eventModel.ActualEndDate == DateTime.MinValue)
            {
                dataRow["EarningsActualEndDate"] = DBNull.Value;
            }
            else
            {
                dataRow["EarningsActualEndDate"] = eventModel.ActualEndDate;
            }

            dataRow["ApprenticeshipId"] = eventModel.ApprenticeshipId ?? DbNull;
            dataRow["ApprenticeshipPriceEpisodeId"] = eventModel.ApprenticeshipPriceEpisodeId ?? DbNull;
            dataRow["NonPaymentReason"] = eventModel.NonPaymentReason ?? DbNull;

            return dataRow;
        }
    }
}