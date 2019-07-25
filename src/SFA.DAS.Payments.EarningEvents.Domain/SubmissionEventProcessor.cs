using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Domain
{
    public interface ISubmissionEventProcessor
    {
        LegacySubmissionEvent ProcessSubmission(SubmittedPriceEpisodeEntity currentEpisode, SubmittedPriceEpisodeEntity lastSeenEpisode);
    }

    public class SubmissionEventProcessor : ISubmissionEventProcessor
    {
        public LegacySubmissionEvent ProcessSubmission(SubmittedPriceEpisodeEntity currentEpisode, SubmittedPriceEpisodeEntity lastSeenEpisode)
        {
            LegacySubmissionEvent @event = null;
            var currentIlr = currentEpisode.IlrDetails;
            var lastSeenIlr = lastSeenEpisode?.IlrDetails;

            // Check for any changes in properties we care about
            if (currentIlr.StandardCode != lastSeenIlr?.StandardCode)
            {
                (@event = new LegacySubmissionEvent()).StandardCode = currentIlr.StandardCode;
            }

            if (currentIlr.ProgrammeType != lastSeenIlr?.ProgrammeType)
            {
                (@event = @event ?? new LegacySubmissionEvent()).ProgrammeType = currentIlr.ProgrammeType;
            }

            if (currentIlr.FrameworkCode != lastSeenIlr?.FrameworkCode)
            {
                (@event = @event ?? new LegacySubmissionEvent()).FrameworkCode = currentIlr.FrameworkCode;
            }

            if (currentIlr.PathwayCode != lastSeenIlr?.PathwayCode)
            {
                (@event = @event ?? new LegacySubmissionEvent()).PathwayCode = currentIlr.PathwayCode;
            }

            if (currentIlr.ActualStartDate != lastSeenIlr?.ActualStartDate)
            {
                (@event = @event ?? new LegacySubmissionEvent()).ActualStartDate = currentIlr.ActualStartDate;
            }

            if (currentIlr.PlannedEndDate != lastSeenIlr?.PlannedEndDate)
            {
                (@event = @event ?? new LegacySubmissionEvent()).PlannedEndDate = currentIlr.PlannedEndDate;
            }

            if (currentIlr.ActualEndDate != lastSeenIlr?.ActualEndDate)
            {
                (@event = @event ?? new LegacySubmissionEvent()).ActualEndDate = currentIlr.ActualEndDate;
            }

            if (currentIlr.OnProgrammeTotalPrice != lastSeenIlr?.OnProgrammeTotalPrice)
            {
                (@event = @event ?? new LegacySubmissionEvent()).OnProgrammeTotalPrice = currentIlr.OnProgrammeTotalPrice;
            }

            if (currentIlr.CompletionTotalPrice != lastSeenIlr?.CompletionTotalPrice)
            {
                (@event = @event ?? new LegacySubmissionEvent()).CompletionTotalPrice = currentIlr.CompletionTotalPrice;
            }

            if (currentIlr.NiNumber != lastSeenIlr?.NiNumber)
            {
                (@event = @event ?? new LegacySubmissionEvent()).NINumber = currentIlr.NiNumber;
            }

            // any difference in these fields should mean an event is created, but as we always set these fields there's no need to set them here
            if (currentIlr.CommitmentId != lastSeenIlr?.CommitmentId
                || currentIlr.EmployerReferenceNumber != lastSeenIlr?.EmployerReferenceNumber
                || currentIlr.EPAOrgId != lastSeenIlr?.EPAOrgId
                || currentIlr.GivenNames != lastSeenIlr?.GivenNames
                || currentIlr.FamilyName != lastSeenIlr?.FamilyName
                || currentIlr.CompStatus != lastSeenIlr?.CompStatus)
            {
                if (@event == null)
                    @event = new LegacySubmissionEvent();
            }

            // If there have been changes then set the standard properties
            if (@event == null)
                return null;

            @event.CommitmentId = currentIlr.CommitmentId;
            @event.IlrFileName = currentIlr.IlrFileName;
            @event.FileDateTime = currentIlr.FileDateTime;
            @event.SubmittedDateTime = currentIlr.SubmittedDateTime;
            @event.ComponentVersionNumber = 2;
            @event.UKPRN = currentEpisode.Ukprn;
            @event.ULN = currentIlr.Uln;
            @event.LearnRefNumber = currentEpisode.LearnRefNumber;
            @event.AimSeqNumber = currentIlr.AimSeqNumber;
            @event.PriceEpisodeIdentifier = currentEpisode.PriceEpisodeIdentifier;
            @event.EmployerReferenceNumber = currentIlr.EmployerReferenceNumber;
            @event.AcademicYear = currentIlr.AcademicYear;
            // EPAOrgId is optional in the ilr, so we need to always set it, otherwise if it is null,
            // the consumer won't know if it hasn't changed or if it's been removed on a subsequent irl submission
            @event.EPAOrgId = currentIlr.EPAOrgId;
            @event.GivenNames = currentIlr.GivenNames;
            @event.FamilyName = currentIlr.FamilyName;
            @event.CommitmentId = currentIlr.CommitmentId;

            @event.StandardCode = currentIlr.StandardCode;
            @event.ActualStartDate = currentIlr.ActualStartDate;
            @event.CompStatus = currentIlr.CompStatus;
            return @event;
        }
    }
}
