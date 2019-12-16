﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class SubmittedLearnerAimBuilder : EarningEventBuilderBase, ISubmittedLearnerAimBuilder
    {
        private readonly IMapper mapper;

        public SubmittedLearnerAimBuilder(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public IList<SubmittedLearnerAimModel> Build(ProcessLearnerCommand processLearnerCommand)
        {
            var intermediateAims = InitialLearnerTransform(processLearnerCommand, null);
            var validIntermediateAims = intermediateAims.Where(x => x.ContractType != ContractType.None);
            return validIntermediateAims.Select(mapper.Map<SubmittedLearnerAimModel>).ToList();
        }
    }
}
