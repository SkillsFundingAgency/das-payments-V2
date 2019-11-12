using AutoFixture;
using AutoFixture.NUnit3;
using System.Reflection;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.AutoFixture
{

    public class NegativeAttribute : CustomizeAttribute
    {
        public override ICustomization GetCustomization(ParameterInfo parameter)
        {
            return new SpecimenCustomization(new RandomNumericSequenceGenerator(-900, -100));
        }
    }
}