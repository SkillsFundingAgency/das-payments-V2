using System;
using AutoMapper;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.DataLock
{
    public interface IDataLockEventMapper
    {
        DataLockEventModel Map(DataLockEvent dataLockEvent);
        DataLockEventModel Map(DataLockEventModel dataLockEventModel);
    }

    public class DataLockEventMapper : IDataLockEventMapper
    {
        private readonly IMapper mapper;

        public DataLockEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public DataLockEventModel Map(DataLockEvent dataLockEvent)
        {
            return mapper.Map<DataLockEventModel>(dataLockEvent);
        }

        public DataLockEventModel Map(DataLockEventModel dataLockEventModel)
        {
            return mapper.Map<DataLockEventModel, DataLockEventModel>(dataLockEventModel);
        }
    }
}