Specks
======

An expression-based .NET implementation of the specification pattern,
providing a framework for defining, combining, and evaluating custom 
domain-specific specifications.

About Specifications
--------------------

The specification pattern is a particular software design pattern whereby 
individual atomic units of business logic can be combined with other units
to provide highly maintainable, customizable and readable code.

Background:

* [Wikipedia](http://en.wikipedia.org/wiki/Specification_pattern)
* [PDF by Evans & Fowler](http://martinfowler.com/apsupp/spec.pdf)


Usage
-----

The library is extensively documented with XML comments, but the following is
a brief overview of the framework itself and how to use it.

### Instantiating Specifications

Creating specification instances can be done in a few ways:

* Instantiating the specification class directly

        var spec = new MySpecification();

* Using the `Specify` or `Specify<T>` fluent API

        Specification<int> spec1 = Specify.GreaterThan(0);
or

        // DateIsLastWeek would be a custom-defined specification
        Specification<DateTime> spec2 = Specify<DateTime>.Where<DateIsLastWeek>();

* Calling composition methods on an existing specification. Note you can use 
the generic syntax if the spec has a default no-arg constructor, otherwise you 
must pass an instance in to the composition method.

        var composite = spec1.And<MySpec>()
                            .Or<MyOtherSpec>()
                            .And(new AnotherSpec());

### Applying Specifications

Once you have an instance of a specification, applying candidate selection
can be done via:

* Invoking the `IsSatisifedBy()` method directly

        if(spec.IsSatisfiedBy(candidate))
            DoSomething();

* Invoking one of the `Filter()` overloads and passing in a collection

        var matches = spec.Filter(collection);

* Passing the specification to a LINQ query using one of the `Matching<T>()`
extension method overloads

        var query1 = collection.Matching(spec);


### Defining Custom Specifications

Custom specifications can be created in the following manners:

* Ad-hoc by using `Specify<T>.Where(Expression<Func<T, bool>>)` and supplying
a lambda expression

        var intGreaterThanZero = Specify<int>.Where(i => i > 0);

* Deriving from `Specification<T>` and implementing the `BuildCriteria()` method
by returning a lambda expression

        public class IntegerGreaterThanZero : Specification<int>
        {
            protected override Expression<Func<int, bool>> BuildCriteria()
            {
                return x => x > 0;
            }            
        }

* Deriving from `CompositeSpecification<T>` and implementing the `BuildComposite()`
method by returning a composite specification instance.

        public class IntegerComposite : CompositeSpecification<int>
        {
            protected override Specification<int> BuildComposite()
            {
                return Specify<int>
                    .Where<IntegerGreaterThanZero>()
                    .Or<IntegerEqualToZero>()
                    .AndNot<IntegerLessThanZero>();
            }
        }


Roadmap
-------

* Finish Subsumption and Partial Fulfillment aspects

* Improve the fluent API a bit, it still seems klunky in some respects.