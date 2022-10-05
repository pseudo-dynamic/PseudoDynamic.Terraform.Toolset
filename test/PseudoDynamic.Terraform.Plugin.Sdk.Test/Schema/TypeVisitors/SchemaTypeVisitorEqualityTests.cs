using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;
using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors;
using PseudoDynamic.Terraform.Plugin.Internals;

namespace PseudoDynamic.Terraform.Plugin.Schema.Visitors
{
    public class SchemaTypeVisitorEqualityTests
    {
        public static IEnumerable<object[]> GetSchemaTypes()
        {
            yield return new object[] {
                typeof(ZeroDepthSchema),
                new SchemaTypeNode(new VisitingContext(typeof(ZeroDepthSchema)) { ContextType = VisitingContextType.Complex })
            };

            yield return new object[] {
                typeof(PropertySchema),
                new SchemaTypeNode(new VisitingContext(typeof(PropertySchema)) { ContextType = VisitingContextType.Complex }) {
                    new SchemaTypeNode(new VisitingContext(typeof(string)) { ContextType = VisitingContextType.Property })
                }
            };

            yield return new object[] {
                typeof(NestedSchema),
                new SchemaTypeNode(new VisitingContext(typeof(NestedSchema)) { ContextType = VisitingContextType.Complex }) {
                    new SchemaTypeNode(new VisitingContext(typeof(PropertySchema)) { ContextType = VisitingContextType.Property }) {
                        new SchemaTypeNode(new VisitingContext(typeof(PropertySchema)) { ContextType = VisitingContextType.Complex }) {
                            new SchemaTypeNode(new VisitingContext(typeof(string)) { ContextType = VisitingContextType.Property })
                        }
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentSchema),
                new SchemaTypeNode(new VisitingContext(typeof(PropertyArgumentSchema)) { ContextType = VisitingContextType.Complex }) {
                    new SchemaTypeNode(new VisitingContext(typeof(List<string>)) { ContextType = VisitingContextType.Property }) {
                        new SchemaTypeNode(new VisitingContext(typeof(string)) { ContextType = VisitingContextType.PropertyGenericArgument })
                    }
                }
            };

            yield return new object[] {
                typeof(PropertyArgumentNestedSchema),
                new SchemaTypeNode(new VisitingContext(typeof(PropertyArgumentNestedSchema)) { ContextType = VisitingContextType.Complex }) {
                    new SchemaTypeNode(new VisitingContext(typeof(IDictionary<string, PropertySchema>)) { ContextType = VisitingContextType.Property }) {
                        new SchemaTypeNode(new VisitingContext(typeof(string)) { ContextType = VisitingContextType.PropertyGenericArgument }),
                        new SchemaTypeNode(new VisitingContext(typeof(PropertySchema)) { ContextType = VisitingContextType.PropertyGenericArgument }) {
                            new SchemaTypeNode(new VisitingContext(typeof(PropertySchema)) { ContextType = VisitingContextType.Complex }) {
                                new SchemaTypeNode(new VisitingContext(typeof(string)) { ContextType = VisitingContextType.Property })
                            }
                        }
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetSchemaTypes))]
        public void Schema_type_survives_equality_checks(Type schemaType, SchemaTypeNode expectedNode)
        {
            var actualNode = new SchemaTypeVisitor().VisitSchema(schemaType);
            Assert.Equal(expectedNode, actualNode, SchemaTypeNodeEqualityComparer.Default);
        }

        [Schema]
        public class ZeroDepthSchema { }

        [Schema]
        public class PropertySchema
        {
            public string String { get; set; }
        }

        [Schema]
        public class NestedSchema
        {
            public PropertySchema Schema { get; set; }
        }

        [Schema]
        public class PropertyArgumentSchema
        {
            public List<string> List { get; set; }
        }

        [Schema]
        public class PropertyArgumentNestedSchema
        {
            public IDictionary<string, PropertySchema> Dictionary { get; set; }
        }
    }
}
