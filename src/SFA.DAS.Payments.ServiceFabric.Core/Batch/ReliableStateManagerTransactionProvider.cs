﻿using Microsoft.ServiceFabric.Data;

namespace SFA.DAS.Payments.ServiceFabric.Core.Batch
{
    public interface IReliableStateManagerTransactionProvider
    {
        ITransaction Current { get; }
    }

    public class ReliableStateManagerTransactionProvider: IReliableStateManagerTransactionProvider
    {
        public ITransaction Current { get; internal set; }
    }
}