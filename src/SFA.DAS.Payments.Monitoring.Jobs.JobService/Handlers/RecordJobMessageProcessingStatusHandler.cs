using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Application.Messaging.Telemetry;
using SFA.DAS.Payments.Monitoring.Jobs.Application;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService.Handlers
{
    public class RecordJobMessageProcessingStatusHandler : IHandleMessages<RecordJobMessageProcessingStatus>
    {
        //private readonly IUnitOfWorkScopeFactory scopeFactory;
        private readonly IJobMessageService jobMessageService;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public RecordJobMessageProcessingStatusHandler(
            //IUnitOfWorkScopeFactory scopeFactory,
            IJobMessageService jobMessageService,
            IPaymentLogger logger,
            ITelemetry telemetry
            )
        {
            //this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.jobMessageService = jobMessageService ?? throw new ArgumentNullException(nameof(jobMessageService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.telemetry = telemetry;
        }

        public Task Handle(RecordJobMessageProcessingStatus message, IMessageHandlerContext context)
        {
            return TelemetryEvent.TrackAsync(telemetry, "JobService.RecordJobMessageProcessingStatus 1.inner", "", async () =>
            {
                //IUnitOfWorkScope scope = null;

                //telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus 2.scope=", "", () =>
                //{
                //    scope = scopeFactory.Create("JobService.RecordJobMessageProcessingStatus");
                //});

                await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus 3.try/catch", "", async () =>
                {
                    try
                    {
                        //IJobMessageService jobMessageService = null;

                        //telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus 4.Resolve<IJobMessageService>", "", () =>
                        //    {
                        //        jobMessageService = scope.Resolve<IJobMessageService>();
                        //    });

                        await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus 5.RecordCompletedJobMessageStatus", "", async () =>
                            {
                                await jobMessageService.RecordCompletedJobMessageStatus(message, CancellationToken.None).ConfigureAwait(false);
                            });

                        //await telemetry.TrackActionAsync("JobService.RecordJobMessageProcessingStatus 6.scope.Commit", "", async () =>
                        //    {
                        //        await scope.Commit();
                        //    });
                    }
                    catch (Exception ex)
                    {
                        //telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus 6.scope.Abort", "", () =>
                        //    {
                        //        scope.Abort();
                        //    });

                        logger.LogWarning($"Failed to record earnings job: {message.JobId}. Error: {ex.Message}, {ex}");
                        throw;
                    }
                    finally
                    {
                        //telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus 7.dispose jobs context", "", () =>
                        //{
                        //    ((JobsDataContext)this.context).Dispose();
                        //});
                        //telemetry.TrackAction("JobService.RecordJobMessageProcessingStatus 8.dispose scope", "", () =>
                        //{
                        //    ((UnitOfWorkScope)scope).Dispose(telemetry);
                        //});
                    }
                });
            });
        }
    }
}