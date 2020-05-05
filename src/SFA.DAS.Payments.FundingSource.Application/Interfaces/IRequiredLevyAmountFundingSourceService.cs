using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IRequiredLevyAmountFundingSourceService
    {
        Task AddRequiredPayment(CalculatedRequiredLevyAmount paymentEvent);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message);
        //Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId);
        Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent);
        Task RemovePreviousSubmissions(long commandJobId, byte collectionPeriod, short academicYear, DateTime commandSubmissionDate, long ukprn);

        Task RemoveCurrentSubmission(long commandJobId, byte collectionPeriod, short academicYear, DateTime commandSubmissionDate, long ukprn);
    }

    public interface ISubmissionCleanUpService
    {
        Task RemovePreviousSubmissions(long commandJobId, byte collectionPeriod, short academicYear, DateTime commandSubmissionDate, long ukprn);
        Task RemoveCurrentSubmission(long commandJobId, byte collectionPeriod, short academicYear, long ukprn);
    }

    public interface IEmployerProviderPriorityStorageService
    {
        Task StoreEmployerProviderPriority(EmployerChangedProviderPriority providerPriorityEvent);
    }

    public interface IFundingSourceEventGenerationService
    {
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId);
    }
    public interface ITransferFundingSourceEventGenerationService
    {
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> ProcessReceiverTransferPayment(ProcessUnableToFundTransferFundingSourcePayment message);
    }

    public interface ICalculatedRequiredLevyAmountPrioritisationService
    {
        Task<List<CalculatedRequiredLevyAmount>> Prioritise(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities);
    }

    public interface IFundingSourcePaymentEventBuilder
    {
        List<FundingSourcePaymentEvent> BuildFundingSourcePaymentsForRequiredPayment(CalculatedRequiredLevyAmount requiredPaymentEvent, long employerAccountId, long jobId);
    }
}