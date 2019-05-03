using System;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Exceptions
{
    public class DcJobNotFoundException : InvalidOperationException
    {
        public DcJobNotFoundException() : base($"Job not found.") { }
        public DcJobNotFoundException(long dcJobId) : base($"Job not found. External Job id: {dcJobId}") { }
        public DcJobNotFoundException(long dcJobId, Exception innerException) : base($"Job not found. External Job id: {dcJobId}", innerException) { }
    }
}