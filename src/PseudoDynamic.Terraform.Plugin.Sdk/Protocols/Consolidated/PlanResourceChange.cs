namespace PseudoDynamic.Terraform.Plugin.Protocols.Consolidated
{
    internal static class PlanResourceChange
    {
        public class Request
        {
            public string TypeName {
                get => _typeName ?? throw new InvalidOperationException("Type name is unexpectedly null");
                set => _typeName = value;
            }

            public DynamicValue? PriorState { get; set; }

            public DynamicValue ProposedNewState {
                get => _proposedNewState ?? throw new InvalidOperationException("Proposed new state is unexpectedly null");
                set => _proposedNewState = value;
            }

            public DynamicValue Config {
                get => _config ?? throw new InvalidOperationException("Config is unexpectedly null");
                set => _config = value;
            }

            public ReadOnlyMemory<byte> PriorPrivate { get; set; }
            public DynamicValue? ProviderMeta { get; set; }

            private string? _typeName;
            private DynamicValue? _config;
            private DynamicValue? _proposedNewState;
        }

        public class Response
        {
            public DynamicValue? PlannedState { get; set; }
            public IList<AttributePath>? RequiresReplace { get; set; }
            public ReadOnlyMemory<byte> PlannedPrivate { get; set; }
            public IList<Diagnostic>? Diagnostics { get; set; }


            // This may be set only by the helper/schema "SDK" in the main Terraform
            // repository, to request that Terraform Core >=0.12 permit additional
            // inconsistencies that can result from the legacy SDK type system
            // and its imprecise mapping to the >=0.12 type system.
            // The change in behavior implied by this flag makes sense only for the
            // specific details of the legacy SDK type system, and are not a general
            // mechanism to avoid proper type handling in providers.
            //
            //     ====              DO NOT USE THIS              ====
            //     ==== THIS MUST BE LEFT UNSET IN ALL OTHER SDKS ====
            //     ====              DO NOT USE THIS              ====
            public bool LegacyTypeSystem { get; set; }
        }
    }
}
