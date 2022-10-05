using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.Visitors
{
    public class SchemaTypeVisitorTests
    {
        [Theory]
        [InlineData(new object[] { typeof(NonAnnotatedSchemas.Default) })]
        public void Schema_type_visitor_throws_exception_due_to_missing_schema_attribute(Type schemaType)
        {
            var error = Assert.IsType<ArgumentException>(Record.Exception(() => new SchemaTypeVisitor().VisitSchema(schemaType)));
            Assert.Contains("must be annotated", error.Message);
        }

        [Fact]
        public void Schema_type_visitor_throws_exception_due_to_open_generic_type()
        {
            var error = Assert.IsType<ArgumentException>(Record.Exception(() => new SchemaTypeVisitor().VisitSchema(typeof(GenericSchema<>))));
            Assert.Contains("closed generic type", error.Message);
        }

        [Theory]
        [InlineData(new object[] { typeof(DependencyCycleSchemas.Property) })]
        [InlineData(new object[] { typeof(DependencyCycleSchemas.PropertyArgument) })]
        [InlineData(new object[] { typeof(DependencyCycleSchemas.Generic<object>) })]
        public void Schema_type_visitor_detects_dependency_cycle(Type schemaType)
        {
            _ = Assert.IsType<TypeDependencyCycleException>(Record.Exception(() => new SchemaTypeVisitor().VisitSchema(schemaType)));
        }

        public class NonAnnotatedSchemas
        {
            public class Default { }

            [Schema]
            public class Property
            {
                public NestedSchema Infinite { get; set; }

                public class NestedSchema { }
            }
        }

        [Schema]
        public class GenericSchema<T> { }

        public static class DependencyCycleSchemas
        {
            [Schema]
            public class Property
            {
                public Property Infinite { get; set; }
            }

            [Schema]
            public class PropertyArgument
            {
                public IList<PropertyArgument> Infinite { get; set; }
            }

            [Schema]
            public class Generic<T>
            {
                public IList<Generic<T>> Infinite { get; set; }
            }
        }
    }
}
