namespace SFA.DAS.Payments.Application.Repositories
{
    public struct ConditionalValue<T>
    {
        public ConditionalValue(bool hasValue, T value)
        {
            HasValue = hasValue;
            Value = value;
        }

        public bool HasValue { get; }

        public T Value { get; }
    }
}