using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    public static class FM36GlobalExtensions
    {
        public static void SetUkprn(this FM36Global fm36Global, int ukprn)
        {
            fm36Global.UKPRN = ukprn;
        }

        public static void SetUlnForLearner(this FM36Global fm36Global, int learnerIndex, long uln)
        {
            fm36Global.Learners[learnerIndex].ULN = uln;
        }
    }
}
