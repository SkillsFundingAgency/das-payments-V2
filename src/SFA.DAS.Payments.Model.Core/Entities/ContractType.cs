using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public enum ContractType: byte
    {
        None = byte.MaxValue, 
        Act1 = 1,
        Act2 = 2,
    }
}