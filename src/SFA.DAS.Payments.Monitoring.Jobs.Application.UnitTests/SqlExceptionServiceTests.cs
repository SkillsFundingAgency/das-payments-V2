using System.Data.SqlClient;
using Autofac.Extras.Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.UnitTests
{
    [TestFixture]
    public class SqlExceptionServiceTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
        }

        [Test]
        public void IsConstraintViolation_Returns_False_If_No_Inner_Exception()
        {
            var service= mocker.Create<SqlExceptionService>();
            var isConstraintViolation =
                service.IsConstraintViolation(new DbUpdateException("Not a violation", (SqlException) null));
            isConstraintViolation.Should().BeFalse();
        }
    }
}