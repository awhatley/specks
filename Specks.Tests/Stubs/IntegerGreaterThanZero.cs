using System;
using System.Linq.Expressions;

namespace Specks.Tests.Stubs
{
    public class IntegerGreaterThanZero : Specification<int>
    {
        protected override Expression<Func<int, bool>> BuildCriteria()
        {
            return x => x > 0;
        }
    }
}