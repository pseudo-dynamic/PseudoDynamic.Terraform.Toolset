﻿namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal class ApplyResourceChange
    {
        public class Request
        {
            public string? TypeName { get; set; }
            public DynamicValue? PriorState { get; set; }
            public DynamicValue? PlannedState { get; set; }
            public DynamicValue? Config { get; set; }
            public IEnumerable<byte>? PlannedPrivate { get; set; }
            public DynamicValue? ProviderMeta { get; set; }
        }

        public class Response
        {
            public DynamicValue? NewState { get; set; }
            public IEnumerable<byte>? Private { get; set; }
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