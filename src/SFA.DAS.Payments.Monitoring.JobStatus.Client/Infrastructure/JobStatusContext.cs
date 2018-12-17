using System;
using System.Collections.Generic;
using Autofac;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure
{
    public class JobStatusContext
    {
        public Guid IncomingEventId { get; set; }
        public List<(DateTimeOffset StartTime, Guid EventId)> GeneratedEvents { get; set; }

        public JobStatusContext()
        {
            GeneratedEvents = new List<(DateTimeOffset StartTime, Guid EventId)>();
        }
    }

    //public class JobStatusContextFactory
    //{
    //    public JobStatusContextFactory(ILifetimeScope )
    //}
}