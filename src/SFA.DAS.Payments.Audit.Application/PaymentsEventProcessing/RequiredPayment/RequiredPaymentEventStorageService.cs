using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.RequiredPayment;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment
{
    public interface IRequiredPaymentEventStorageService
    {
        Task StoreRequiredPayments(List<RequiredPaymentEventModel> models, CancellationToken cancellationToken);
    }

    public class RequiredPaymentEventStorageService : IRequiredPaymentEventStorageService
    {
        private readonly IRequiredPaymentEventMapper mapper;
        private readonly IPaymentLogger logger;
        private readonly IRequiredPaymentEventRepository repository;

        public RequiredPaymentEventStorageService(IRequiredPaymentEventMapper mapper, IPaymentLogger logger, IRequiredPaymentEventRepository repository)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StoreRequiredPayments(List<RequiredPaymentEventModel> models, CancellationToken cancellationToken)
        {
            try
            {
                //TODO: remove duplicates in this batch
                await repository.SaveRequiredPaymentEvents(models, cancellationToken);
            }
            catch (Exception e)
            {
                if (!e.IsUniqueKeyConstraintException() && !e.IsDeadLockException()) throw;
                logger.LogInfo($"Batch contained a duplicate required payment.  Will store each individually and discard duplicate.");
                await repository.SaveRequiredPaymentEventsIndividually(models.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}