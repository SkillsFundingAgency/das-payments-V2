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

    public static class ConditionalValue
    {
        public static ConditionalValue<T> With<T>(T t) => new ConditionalValue<T>(true, t);

        public static ConditionalValue<T[]> WithArray<T>(params T[] t) => new ConditionalValue<T[]>(true, t);
    }
}