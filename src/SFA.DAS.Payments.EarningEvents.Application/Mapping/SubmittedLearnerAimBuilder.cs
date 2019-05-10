using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class SubmittedLearnerAimBuilder : EarningEventBuilderBase
    {
        private readonly IMapper mapper;

        public SubmittedLearnerAimBuilder(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IList<SubmittedLearnerAimModel> Build(ProcessLearnerCommand processLearnerCommand)
        {
            var intermediateAims = InitialLearnerTransform(processLearnerCommand, null);
            return intermediateAims.Select(mapper.Map<SubmittedLearnerAimModel>).ToList();
        }
    }
}
