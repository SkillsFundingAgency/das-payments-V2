using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Exceptions;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface IProviderPaymentStorageService
    {
        Task StoreProviderPayments(List<PaymentModel> payments, CancellationToken cancellationToken);
    }


    public class ProviderPaymentStorageService: IProviderPaymentStorageService
    {
        private readonly IPaymentLogger logger;
        private readonly IProviderPaymentMapper mapper;
        private readonly IProviderPaymentsRepository repository;

        public ProviderPaymentStorageService(IPaymentLogger logger, IProviderPaymentMapper mapper, IProviderPaymentsRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StoreProviderPayments(List<PaymentModel> payments, CancellationToken cancellationToken)
        {
            try
            {
                await repository.SavePayments(payments, cancellationToken);
            }
            catch (DuplicatePaymentException e)
            {
                logger.LogInfo($"Batch contained a duplicate payment.  Will store each payment individually and discard duplicate(s).");
                await repository.SavePaymentsIndividually(payments.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}