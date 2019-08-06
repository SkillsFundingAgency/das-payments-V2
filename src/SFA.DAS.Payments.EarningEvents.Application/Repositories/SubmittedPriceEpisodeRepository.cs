using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Repositories
{
    public interface ISubmittedPriceEpisodeRepository
    {
        Task<List<SubmittedPriceEpisodeEntity>> GetSubmittedPriceEpisodes(long ukprn, string learnRefNumber, CancellationToken cancellationToken);
    }

    public class SubmittedPriceEpisodeRepository : ISubmittedPriceEpisodeRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;
        private readonly IPaymentLogger logger;

        public SubmittedPriceEpisodeRepository(IPaymentsDataContext paymentsDataContext, IPaymentLogger logger)
        {
            this.paymentsDataContext = paymentsDataContext;
            this.logger = logger;
        }

        public async Task<List<SubmittedPriceEpisodeEntity>> GetSubmittedPriceEpisodes(long ukprn, string learnRefNumber, CancellationToken cancellationToken)
        {
            var entities = await paymentsDataContext.SubmittedPriceEpisode.Where(e => e.Ukprn == ukprn && e.LearnerReferenceNumber == learnRefNumber)
                .Select(e => new SubmittedPriceEpisodeEntity
                {
                    Id = e.Id,
                    Ukprn = e.Ukprn,
                    LearnRefNumber = e.LearnerReferenceNumber,
                    PriceEpisodeIdentifier = e.PriceEpisodeIdentifier,
                    IlrDetails = JsonConvert.DeserializeObject<IlrDetails>(e.IlrDetails)
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            logger.LogDebug($"retrieved {entities.Count} submitted price episodes for UKPRN {ukprn}");

            return entities;
        }
    }
}
