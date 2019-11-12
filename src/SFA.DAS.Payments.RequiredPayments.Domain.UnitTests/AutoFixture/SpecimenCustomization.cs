using AutoFixture;
using AutoFixture.Kernel;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.AutoFixture
{
    public class SpecimenCustomization : ICustomization
    {
        private readonly ISpecimenBuilder builder;

        public SpecimenCustomization(ISpecimenBuilder builder) => this.builder = builder;

        public void Customize(IFixture fixture) => fixture.Customizations.Add(builder);
    }
}