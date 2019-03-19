using System;
using System.Linq.Expressions;
using AutoMapper;

namespace SFA.DAS.Payments.RequiredPayments.Application.Extensions
{
    public static class AutomapperExtensions
    {
        public static IMappingExpression<T1, T2> Ignore<T1, T2, TMember>(this IMappingExpression<T1, T2> source,
            Expression<Func<T2,TMember>> memberToIgnore)
        {
            return source.ForMember(memberToIgnore, opt => opt.Ignore());
        }
    }
}