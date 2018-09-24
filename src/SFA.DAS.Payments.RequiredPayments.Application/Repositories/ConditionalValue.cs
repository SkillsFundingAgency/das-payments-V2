namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    // borrowed from ServiceFabric
    public struct ConditionalValue<T>
    {
        public ConditionalValue(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.Value = value;
        }

        public bool HasValue { get; }

        public T Value { get; }
    }
}