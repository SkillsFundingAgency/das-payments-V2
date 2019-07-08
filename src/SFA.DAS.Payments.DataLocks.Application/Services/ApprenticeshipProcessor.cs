using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NServiceBus;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IApprenticeshipProcessor
    {
        Task Process(ApprenticeshipCreatedEvent createdEvent);
        Task ProcessUpdatedApprenticeship(ApprenticeshipUpdatedApprovedEvent updatedEvent);
    }

    public class ApprenticeshipProcessor : IApprenticeshipProcessor
    {
        private readonly IPaymentLogger logger;
        private readonly IMapper mapper;
        private readonly IApprenticeshipService apprenticeshipService;
        private readonly IEndpointInstanceFactory endpointInstanceFactory;

        public ApprenticeshipProcessor(IPaymentLogger logger, IMapper mapper, IApprenticeshipService apprenticeshipService, IEndpointInstanceFactory endpointInstanceFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.apprenticeshipService = apprenticeshipService ?? throw new ArgumentNullException(nameof(apprenticeshipService));
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
        }

        public async Task Process(ApprenticeshipCreatedEvent createdEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
                var model = mapper.Map<ApprenticeshipModel>(createdEvent);
                var duplicates = await apprenticeshipService.NewApprenticeship(model).ConfigureAwait(false);
                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(model);
                updatedEvent.Duplicates = duplicates.Select(duplicate => new ApprenticeshipDuplicate { Ukprn = duplicate.Ukprn, ApprenticeshipId = duplicate.ApprenticeshipId }).ToList();
                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);
                logger.LogInfo($"Finished processing the apprenticeship created event. Apprenticeship id: {createdEvent.ApprenticeshipId}, employer account id: {createdEvent.AccountId}, Ukprn: {createdEvent.ProviderId}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }

        public async Task ProcessUpdatedApprenticeship(ApprenticeshipUpdatedApprovedEvent apprenticeshipApprovedEvent)
        {
            try
            {
                logger.LogDebug($"Now processing the apprenticeship update event. Apprenticeship id: {apprenticeshipApprovedEvent.ApprenticeshipId}");
                var model = mapper.Map<UpdatedApprenticeshipModel>(apprenticeshipApprovedEvent);
                var ipdateApprenticeship = await apprenticeshipService.UpdateApprenticeship(model).ConfigureAwait(false);


                var updatedEvent = mapper.Map<ApprenticeshipUpdated>(model);
                updatedEvent.Duplicates = duplicates.Select(duplicate => new ApprenticeshipDuplicate { Ukprn = duplicate.Ukprn, ApprenticeshipId = duplicate.ApprenticeshipId }).ToList();
                var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                await endpointInstance.Publish(updatedEvent).ConfigureAwait(false);
                logger.LogInfo($"Finished processing the apprenticeship created event. Apprenticeship id: {updatedEvent.ApprenticeshipId}, employer account id: {updatedEvent.AccountId}, Ukprn: {updatedEvent.ProviderId}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing the apprenticeship event. Error: {ex.Message}", ex);
                throw;
            }
        }


    }
}