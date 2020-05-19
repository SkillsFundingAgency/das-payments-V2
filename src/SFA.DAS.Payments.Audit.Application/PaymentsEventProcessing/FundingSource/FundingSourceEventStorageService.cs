using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.Application.Data.FundingSource;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public interface IFundingSourceEventStorageService
    {
        Task StoreFundingSourceEvents(List<FundingSourceEventModel> models, CancellationToken cancellationToken);
    }

    public class FundingSourceEventStorageService : IFundingSourceEventStorageService
    {
        private readonly IFundingSourceEventMapper mapper;
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceEventRepository repository;

        public FundingSourceEventStorageService(IFundingSourceEventMapper mapper, IPaymentLogger logger, IFundingSourceEventRepository repository)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task StoreFundingSourceEvents(List<FundingSourceEventModel> models, CancellationToken cancellationToken)
        {
            try
            {
                //TODO: remove duplicates in this batch
                await repository.SaveSaveFundingSourceEvents(models, cancellationToken);
            }
            catch (Exception e)
            {
                if (!e.IsUniqueKeyConstraintException() && !e.IsDeadLockException()) throw;
                logger.LogInfo($"Batch contained a duplicate required payment.  Will store each individually and discard duplicate.");
                await repository.SaveFundingSourceEventsIndividually(models.Select(model => mapper.Map(model)).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}