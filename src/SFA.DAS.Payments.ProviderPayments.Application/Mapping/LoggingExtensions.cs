using System.Text;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{
    public static class LoggingExtensions
    {
        public static string ToLogString(this FundingSourcePaymentEvent source)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Funding Source EventId: {source.EventId}");
            stringBuilder.AppendLine($"Funding Source Job Id: {source.JobId}");
            stringBuilder.AppendLine($"Funding Source Type: {source.FundingSourceType:G}");
            stringBuilder.AppendLine($"Required Payment Event Id: {source.RequiredPaymentEventId}");
            stringBuilder.AppendLine($"SFA Contribution Percentage: {source.SfaContributionPercentage}");
            stringBuilder.AppendLine($"Transaction Type: {source.TransactionType}");
            stringBuilder.AppendLine($"Account Id: {source.AccountId}");
            stringBuilder.AppendLine($"Amount Due: {source.AmountDue}");
            stringBuilder.AppendLine($"Apprenticeship Employer Type: {source.ApprenticeshipEmployerType}");
            stringBuilder.AppendLine($"Apprenticeship Id: {source.ApprenticeshipId}");
            stringBuilder.AppendLine($"Apprenticeship Price Episode Id: {source.ApprenticeshipPriceEpisodeId}");
            stringBuilder.AppendLine($"Actual End Date: {source.ActualEndDate}");
            stringBuilder.AppendLine($"{nameof(source.CollectionPeriod)}: {source.CollectionPeriod.Period}");
            stringBuilder.AppendLine($"{nameof(source.CompletionAmount)}: {source.CompletionAmount}");
            stringBuilder.AppendLine($"{nameof(source.CompletionStatus)}: {source.CompletionStatus}");
            stringBuilder.AppendLine($"{nameof(source.ContractType)}: {source.ContractType}");
            stringBuilder.AppendLine($"{nameof(source.DeliveryPeriod)}: {source.DeliveryPeriod}");
            stringBuilder.AppendLine($"{nameof(source.EarningEventId)}: {source.EarningEventId}");
            stringBuilder.AppendLine($"{nameof(source.IlrFileName)}: {source.IlrFileName}");
            stringBuilder.AppendLine($"{nameof(source.InstalmentAmount)}: {source.InstalmentAmount}");
            stringBuilder.AppendLine($"{nameof(source.Learner.ReferenceNumber)}: {source.Learner.ReferenceNumber}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.Reference)}: {source.LearningAim.Reference}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.FrameworkCode)}: {source.LearningAim.FrameworkCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.PathwayCode)}: {source.LearningAim.PathwayCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.ProgrammeType)}: {source.LearningAim.ProgrammeType}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.StandardCode)}: {source.LearningAim.StandardCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.SequenceNumber)}: {source.LearningAim.SequenceNumber}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.StartDate)}: {source.LearningAim.StartDate}");

            stringBuilder.AppendLine($"{nameof(source.LearningStartDate)}: {source.LearningStartDate}");
            stringBuilder.AppendLine($"{nameof(source.NumberOfInstalments)}: {source.NumberOfInstalments}");
            stringBuilder.AppendLine($"{nameof(source.NumberOfInstalments)}: {source.NumberOfInstalments}");
            stringBuilder.AppendLine($"{nameof(source.PriceEpisodeIdentifier)}: {source.PriceEpisodeIdentifier}");
            stringBuilder.AppendLine($"{nameof(source.ReportingAimFundingLineType)}: {source.ReportingAimFundingLineType}");
            stringBuilder.AppendLine($"{nameof(source.StartDate)}: {source.StartDate}");
            stringBuilder.AppendLine($"{nameof(source.TransferSenderAccountId)}: {source.TransferSenderAccountId}");
            stringBuilder.AppendLine("");


            return stringBuilder.ToString();
        }

        public static string ToLogString(this ProviderPaymentEvent source)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Funding Source EventId: {source.EventId}");
            stringBuilder.AppendLine($"Funding Source Job Id: {source.JobId}");
            stringBuilder.AppendLine($"Funding Source Type: {source.FundingSourceType:G}");
            stringBuilder.AppendLine($"SFA Contribution Percentage: {source.SfaContributionPercentage}");
            stringBuilder.AppendLine($"Transaction Type: {source.TransactionType}");
            stringBuilder.AppendLine($"Account Id: {source.AccountId}");
            stringBuilder.AppendLine($"Amount Due: {source.AmountDue}");
            stringBuilder.AppendLine($"Apprenticeship Employer Type: {source.ApprenticeshipEmployerType}");
            stringBuilder.AppendLine($"Apprenticeship Id: {source.ApprenticeshipId}");
            stringBuilder.AppendLine($"Apprenticeship Price Episode Id: {source.ApprenticeshipPriceEpisodeId}");
            stringBuilder.AppendLine($"Actual End Date: {source.ActualEndDate}");
            stringBuilder.AppendLine($"{nameof(source.CollectionPeriod)}: {source.CollectionPeriod.Period}");
            stringBuilder.AppendLine($"{nameof(source.CompletionAmount)}: {source.CompletionAmount}");
            stringBuilder.AppendLine($"{nameof(source.CompletionStatus)}: {source.CompletionStatus}");
            stringBuilder.AppendLine($"{nameof(source.ContractType)}: {source.ContractType}");
            stringBuilder.AppendLine($"{nameof(source.DeliveryPeriod)}: {source.DeliveryPeriod}");
            stringBuilder.AppendLine($"{nameof(source.EarningEventId)}: {source.EarningEventId}");
            stringBuilder.AppendLine($"{nameof(source.IlrFileName)}: {source.IlrFileName}");
            stringBuilder.AppendLine($"{nameof(source.InstalmentAmount)}: {source.InstalmentAmount}");
            stringBuilder.AppendLine($"{nameof(source.Learner.ReferenceNumber)}: {source.Learner.ReferenceNumber}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.Reference)}: {source.LearningAim.Reference}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.FrameworkCode)}: {source.LearningAim.FrameworkCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.PathwayCode)}: {source.LearningAim.PathwayCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.ProgrammeType)}: {source.LearningAim.ProgrammeType}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.StandardCode)}: {source.LearningAim.StandardCode}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.SequenceNumber)}: {source.LearningAim.SequenceNumber}");
            stringBuilder.AppendLine($"{nameof(source.LearningAim.StartDate)}: {source.LearningAim.StartDate}");

            stringBuilder.AppendLine($"{nameof(source.LearningStartDate)}: {source.LearningStartDate}");
            stringBuilder.AppendLine($"{nameof(source.NumberOfInstalments)}: {source.NumberOfInstalments}");
            stringBuilder.AppendLine($"{nameof(source.NumberOfInstalments)}: {source.NumberOfInstalments}");
            stringBuilder.AppendLine($"{nameof(source.PriceEpisodeIdentifier)}: {source.PriceEpisodeIdentifier}");
            stringBuilder.AppendLine($"{nameof(source.ReportingAimFundingLineType)}: {source.ReportingAimFundingLineType}");
            stringBuilder.AppendLine($"{nameof(source.StartDate)}: {source.StartDate}");
            stringBuilder.AppendLine($"{nameof(source.TransferSenderAccountId)}: {source.TransferSenderAccountId}");
            stringBuilder.AppendLine("");


            return stringBuilder.ToString();
        }
    }
}

    










