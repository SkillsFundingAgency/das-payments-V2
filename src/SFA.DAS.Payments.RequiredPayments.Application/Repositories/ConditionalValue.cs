namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    // borrowed from ServiceFabric
    public struct ConditionalValue<T>
    {
        private readonly bool _hasValue;
        private readonly T _value;

        public ConditionalValue(bool hasValue, T value)
        {
            _hasValue = hasValue;
            _value = value;
        }

        public bool HasValue => _hasValue;

        public T Value => _value;
    }
}