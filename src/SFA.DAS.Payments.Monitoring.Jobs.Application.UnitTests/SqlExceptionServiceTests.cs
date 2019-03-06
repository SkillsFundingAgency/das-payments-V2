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


        //TODO: Find way to properly mock sqlexception. (No public constructor and class is sealed)
        //[TestCase(2627)]
        //[TestCase(547)]
        //[TestCase(2601)]
        //public void IsConstraintViolation_Returns_True_If_SqlException_Number_Refers_To_Constraint_Error(int sqlExceptionNumber)
        //{
        //    var service= mocker.Create<SqlExceptionService>();
        //    var mockSqlException = mocker.Mock<SqlException>();
        //    mockSqlException.Setup(ex => ex.Number).Returns(sqlExceptionNumber);
        //    var isConstraintViolation =
        //        service.IsConstraintViolation(new DbUpdateException("Not a violation", mockSqlException.Object));
        //    isConstraintViolation.Should().BeTrue();
        //}

        //[Test]
        //public void IsConstraintViolation_Returns_False_If_SqlException_Number_Does_Not_Refer_To_Constraint_Error()
        //{
        //    var service= mocker.Create<SqlExceptionService>();
        //    var mockSqlException = mocker.Mock<SqlException>();
        //    mockSqlException.Setup(ex => ex.Number).Returns(1);
        //    var isConstraintViolation =
        //        service.IsConstraintViolation(new DbUpdateException("Not a violation", mockSqlException.Object));
        //    isConstraintViolation.Should().BeFalse();
        //}
    }
}