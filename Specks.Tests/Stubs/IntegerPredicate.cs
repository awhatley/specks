using System;
using System.Linq.Expressions;

namespace Specks.Tests.Stubs
{
    public class IntegerPredicate : Specification<int>
    {
        private readonly Expression<Func<int, bool>> _criteria;
        
        public IntegerPredicate(Expression<Func<int, bool>> criteria)
        {
            _criteria = criteria;
        }

        protected override Expression<Func<int, bool>> BuildCriteria()
        {
            return _criteria;
        }
    }
}