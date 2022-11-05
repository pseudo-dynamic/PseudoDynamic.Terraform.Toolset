using FluentAssertions;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.ComplexType
{
    public class TypeDependencyCircleTests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="complexType"></param>
        /// <param name="affectedType"></param>
        /// <remarks>
        /// Pure struct type cycle is permitted at compiler-level, so we don't need to test this.
        /// </remarks>
        [Theory]
        [InlineData(typeof(SelfReferenceClass), typeof(SelfReferenceClass))]
        [InlineData(typeof(HavingSelfReferenceStruct), typeof(SelfReferenceStruct))]
        public void Visitor_should_throw_type_dependency_circle_error(Type complexType, Type affectedType) =>
            ComplexTypeVisitor.Default.Invoking(x => x.RewriteThenVisitComplex(complexType)).Should().Throw<TypeDependencyCycleException>()
            .And.Type.Should().Be(affectedType);

        [Theory]
        [InlineData(typeof(Parent))]
        public void Visitor_should_not_throw_type_dependency_circle_error(Type complexType) =>
            ComplexTypeVisitor.Default.Invoking(x => x.RewriteThenVisitComplex(complexType)).Should().NotThrow();

        class SelfReferenceClass
        {
            public SelfReferenceClass? Invalid { get; set; }
        }

        class HavingSelfReferenceStruct
        {
            public SelfReferenceStruct Invalid { get; set; }
        }

        class HavingSelfReferenceStructToo
        {
            public SelfReferenceStruct Invalid { get; set; }
        }

        struct SelfReferenceStruct
        {
            public SelfReferenceStruct()
            {
            }

            public HavingSelfReferenceStructToo? Invalid { get; set; }
        }

        class Parent
        {
            public Child? ChildOne { get; set; }
            public Child? ChildTwo { get; set; }

            internal class Child
            {
            }
        }
    }
}
