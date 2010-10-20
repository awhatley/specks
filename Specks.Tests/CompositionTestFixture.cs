using System;
using System.Linq.Expressions;

using NUnit.Framework;

using Specks.Expressions;
using Specks.Logic;
using Specks.Tests.Stubs;

namespace Specks.Tests
{
    [TestFixture]
    public class CompositionTestFixture
    {
        [Test]
        public void NegationOperator_ReturnsNegation()
        {
            var spec = new IntegerGreaterThanZero();
            var negatedSpec = !spec;

            Assert.That(negatedSpec, Is.TypeOf<Negation<int>>());
            Assert.That(negatedSpec, Has.Property("Inner").EqualTo(spec));
        }

        [Test]
        public void Not_ReturnsNegation()
        {
            var spec = new IntegerGreaterThanZero();
            var negatedSpec = spec.IsFalse();

            Assert.That(negatedSpec, Is.TypeOf<Negation<int>>());
            Assert.That(negatedSpec, Has.Property("Inner").EqualTo(spec));
        }

        [Test]
        public void AndOperator_ReturnsConjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1 & spec2;

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void And_ReturnsConjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.And(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void AndConvert_ReturnsConjunctionWithConversion()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = Specify.EqualTo(DateTime.Now);
            var converter = (Expression<Func<DateTime, int>>)(dt => dt.Minute);
            var conjoinedSpec = spec2.And(converter, spec1);

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec2));
            Assert.That(conjoinedSpec, Has.Property("Right").InstanceOf<Conversion<DateTime, int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Converter").EqualTo(converter));
        }

        [Test]
        public void AndGeneric_ReturnsConjunction()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.And<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void AndNot_ReturnsConjunction_WithNegationRight()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.AndNot(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").EqualTo(spec2));
        }

        [Test]
        public void AndNotConvert_ReturnsConjunctionWithNegatedConversion()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = Specify.EqualTo(DateTime.Now);
            var converter = (Expression<Func<DateTime, int>>)(dt => dt.Minute);
            var conjoinedSpec = spec2.AndNot(converter, spec1);

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec2));
            Assert.That(conjoinedSpec, Has.Property("Right").InstanceOf<Negation<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").InstanceOf<Conversion<DateTime, int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").With.Property("Converter").EqualTo(converter));
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").With.Property("Inner").EqualTo(spec1));
        }

        [Test]
        public void AndNotGeneric_ReturnsConjunction_WithNegationRight()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.AndNot<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<Conjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void OrOperator_ReturnsDisjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1 | spec2;

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void Or_ReturnsDisjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.Or(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void OrConvert_ReturnsDisjunctionWithConversion()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = Specify.EqualTo(DateTime.Now);
            var converter = (Expression<Func<DateTime, int>>)(dt => dt.Minute);
            var conjoinedSpec = spec2.Or(converter, spec1);

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec2));
            Assert.That(conjoinedSpec, Has.Property("Right").InstanceOf<Conversion<DateTime, int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Converter").EqualTo(converter));
        }

        [Test]
        public void OrGeneric_ReturnsDisjunction()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.Or<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void OrNotConvert_ReturnsDisjunctionWithNegatedConversion()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = Specify.EqualTo(DateTime.Now);
            var converter = (Expression<Func<DateTime, int>>)(dt => dt.Minute);
            var conjoinedSpec = spec2.OrNot(converter, spec1);

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec2));
            Assert.That(conjoinedSpec, Has.Property("Right").InstanceOf<Negation<DateTime>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").InstanceOf<Conversion<DateTime, int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").With.Property("Converter").EqualTo(converter));
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").With.Property("Inner").EqualTo(spec1));
        }

        [Test]
        public void OrNot_ReturnsDisjunction_WithNegationRight()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.OrNot(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").EqualTo(spec2));
        }

        [Test]
        public void OrNotGeneric_ReturnsDisjunction_WithNegationRight()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.OrNot<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<Disjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void XorOperator_ReturnsExclusiveDisjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1 ^ spec2;

            Assert.That(conjoinedSpec, Is.TypeOf<ExclusiveDisjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void Xor_ReturnsExclusiveDisjunction()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.Xor(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<ExclusiveDisjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").EqualTo(spec2));
        }

        [Test]
        public void XorGeneric_ReturnsExclusiveDisjunction()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.Xor<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<ExclusiveDisjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void XorNot_ReturnsExclusiveDisjunction_WithNegationRight()
        {
            var spec1 = new IntegerGreaterThanZero();
            var spec2 = new IntegerLessThanZero();
            var conjoinedSpec = spec1.XorNot(spec2);

            Assert.That(conjoinedSpec, Is.TypeOf<ExclusiveDisjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec1));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").EqualTo(spec2));
        }

        [Test]
        public void XorNotGeneric_ReturnsExclusiveDisjunction_WithNegationRight()
        {
            var spec = new IntegerGreaterThanZero();
            var conjoinedSpec = spec.XorNot<IntegerLessThanZero>();

            Assert.That(conjoinedSpec, Is.TypeOf<ExclusiveDisjunction<int>>());
            Assert.That(conjoinedSpec, Has.Property("Left").EqualTo(spec));
            Assert.That(conjoinedSpec, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(conjoinedSpec, Has.Property("Right").With.Property("Inner").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void ForAny_ReturnsExistentialQuantification()
        {
            var spec = new IntegerGreaterThanZero();
            var anySpec = spec.ForAny();

            Assert.That(anySpec, Is.TypeOf<ExistentialQuantification<int>>());
            Assert.That(anySpec, Has.Property("Inner").EqualTo(spec));
        }

        [Test]
        public void ForAll_ReturnsUniversalQuantification()
        {
            var spec = new IntegerGreaterThanZero();
            var anySpec = spec.ForAll();

            Assert.That(anySpec, Is.TypeOf<UniversalQuantification<int>>());
            Assert.That(anySpec, Has.Property("Inner").EqualTo(spec));
        }

        [Test]
        public void ForOne_ReturnsUniqueQuantification()
        {
            var spec = new IntegerGreaterThanZero();
            var anySpec = spec.ForOne();

            Assert.That(anySpec, Is.TypeOf<UniqueQuantification<int>>());
            Assert.That(anySpec, Has.Property("Inner").EqualTo(spec));
        }

        [Test]
        public void Convert_ReturnsConversion()
        {
            var spec = new IntegerGreaterThanZero();
            var converter = (Expression<Func<double, int>>)(d => (int)d);
            var convertedSpec = spec.From(converter);

            Assert.That(convertedSpec, Is.TypeOf<Conversion<double, int>>());
            Assert.That(convertedSpec, Has.Property("Inner").EqualTo(spec));
            Assert.That(convertedSpec, Has.Property("Converter").EqualTo(converter));
        }

        [Test]
        public void Composite_Specification()
        {
            var spec = new IntegerComposite();
            var composite = spec.GetComposite();

            Assert.That(composite, Is.TypeOf<Conjunction<int>>());
            Assert.That(composite, Has.Property("Left").TypeOf<Disjunction<int>>());
            Assert.That(composite, Has.Property("Left").With.Property("Left").TypeOf<IntegerGreaterThanZero>());
            Assert.That(composite, Has.Property("Left").With.Property("Right").TypeOf<IntegerEqualToZero>());
            Assert.That(composite, Has.Property("Right").TypeOf<Negation<int>>());
            Assert.That(composite, Has.Property("Right").With.Property("Inner").TypeOf<IntegerLessThanZero>());
        }

        [Test]
        public void Composite_Criteria()
        {
            var spec = new IntegerComposite();
            var composite = Specify<int>
                .Where<IntegerGreaterThanZero>()
                .Or<IntegerEqualToZero>()
                .AndNot<IntegerLessThanZero>();
            var criteria = spec.Criteria;

            Assert.That(composite.Criteria.ToString(), Is.EqualTo(criteria.ToString()));

            ExpressionWriter.Write(criteria);
        }
    }
}