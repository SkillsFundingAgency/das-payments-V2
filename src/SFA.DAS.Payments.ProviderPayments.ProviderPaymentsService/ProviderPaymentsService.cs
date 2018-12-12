using Microsoft.ServiceFabric.Actors.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using System.Fabric;
using Autofac;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing;
using SFA.DAS.Payments.Audit.Application.ServiceFabric.Infrastructure;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    [StatePersistence(StatePersistence.Persisted)]
    public class ProviderPaymentsStatefulService : AuditStatefulService<ProviderPaymentEventModel>
    {
        public ProviderPaymentsStatefulService(StatefulServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope, IPaymentsEventModelBatchService<ProviderPaymentEventModel> batchService) : base(context, logger, lifetimeScope, batchService)
        {
        }
    }


    //[StatePersistence(StatePersistence.Volatile)]
    //public class ProviderPaymentsService : Actor, Interfaces.IProviderPaymentsService
    //{
    //    private readonly ActorId actorId;
    //    private readonly IProviderPaymentsRepository providerPaymentsRepository;
    //    private readonly IValidateIlrSubmission validateIlrSubmission;
    //    private readonly IPaymentLogger paymentLogger;
    //    private IProviderPaymentsService paymentsService;
    //    private IHandleIlrSubmissionService handleIlrSubmissionService;
    //    private IMonthEndService monthEndService;
    //    private long ukprn;

    //    public ProviderPaymentsService(ActorService actorService, ActorId actorId,
    //        IProviderPaymentsRepository providerPaymentsRepository,
    //        IValidateIlrSubmission validateIlrSubmission,
    //        IPaymentLogger paymentLogger)
    //        : base(actorService, actorId)
    //    {
    //        this.actorId = actorId;
    //        this.providerPaymentsRepository = providerPaymentsRepository ?? throw new ArgumentNullException(nameof(providerPaymentsRepository));
    //        this.validateIlrSubmission = validateIlrSubmission ?? throw new ArgumentNullException(nameof(validateIlrSubmission));
    //        this.paymentLogger = paymentLogger ?? throw new ArgumentNullException(nameof(paymentLogger));
    //    }

    //    public async Task HandlePayment(PaymentModel message, CancellationToken cancellationToken)
    //    {
    //        await paymentsService.ProcessPayment(message, cancellationToken);
    //    }

    //    public async Task<List<PaymentModel>> GetMonthEndPayments(short collectionYear, byte collectionPeriod, CancellationToken cancellationToken)
    //    {
    //        return await monthEndService.GetMonthEndPayments(collectionYear, collectionPeriod, ukprn, cancellationToken);
    //    }

    //    public async Task HandleIlrSubMission(IlrSubmittedEvent message, CancellationToken cancellationToken)
    //    {
    //        await handleIlrSubmissionService.Handle(message, cancellationToken);
    //    }

    //    protected override async Task OnActivateAsync()
    //    {
    //        if (!long.TryParse(actorId.GetStringId(), out ukprn)) throw new Exception("Unable to cast Actor Id to Ukprn");

    //        var reliableCollectionCache = new ReliableCollectionCache<IlrSubmittedEvent>(StateManager);
    //        paymentsService = new Application.Services.ProviderPaymentsService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
    //        handleIlrSubmissionService = new HandleIlrSubmissionService(providerPaymentsRepository, reliableCollectionCache, validateIlrSubmission, paymentLogger);
    //        monthEndService = new MonthEndService(providerPaymentsRepository);
    //        await base.OnActivateAsync().ConfigureAwait(false);
    //    }
    //}
}
