namespace Specks.Tests.Stubs
{
    public class IntegerComposite : CompositeSpecification<int>
    {
        protected override Specification<int> BuildComposite()
        {
            return Specify<int>
                .Where<IntegerGreaterThanZero>()
                .Or<IntegerEqualToZero>()
                .AndNot<IntegerLessThanZero>();
        }

        public Specification<int> GetComposite()
        {
            return BuildComposite();
        }
    }
}