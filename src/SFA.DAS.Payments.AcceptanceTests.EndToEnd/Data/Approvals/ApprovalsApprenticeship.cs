﻿using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data.Approvals
{
    public class ApprovalsApprenticeship
    {
        public long Id { get; set; }
        public string Identifier { get; set; }
        public string CreatedOnDate { get; set; }
        public string AgreedOnDate { get; set; }
        public string Learner { get; set; }
        public string Provider { get; set; }
        public string Employer { get; set; }
        public string SendingEmployer { get; set; }
        public int StandardCode { get; set; }
        public int ProgrammeType { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        public string StartDate { get; set; }
        public string PauseOnDate { get; set; }
        public string EndDate { get; set; }
        public string StoppedOnDate { get; set; }
        public string ResumedOnDate { get; set; }

        public string EmployerType { get; set; }

        public string Status { get; set; }
        public class PriceEpisode
        {
            public string Apprenticeship { get; set; }
            public decimal AgreedPrice { get; set; }
            public string EffectiveFrom { get; set; }
            public string EffectiveTo { get; set; }
        }

        public List<PriceEpisode> PriceEpisodes { get; set; }

        public ApprovalsApprenticeship()
        {
            PriceEpisodes = new List<PriceEpisode>();
        }
    }

    public class ApprovalsApprenticeshipChangeModel
    {
        public long Id { get; set; }
        public string Identifier { get; set; }
        public string StoppedOnDate { get; set; }
        public string PausedOnDate { get; set; }
        public string ResumedOnDate { get; set; }
        public string Status { get; set; }
    }

    public class ApprovalsApprenticeshipPausedModel
    {
        public long Id { get; set; }
        public string Apprenticeship { get; set; }
        public string PausedOnDate { get; set; }
     
    }
}