namespace PseudoDynamic.Terraform.Plugin.Protocols.Models
{
    internal static class UpgradeResourceState
    {
        public class Request
        {
            public string? TypeName { get; set; }

            // version is the schema_version number recorded in the state file
            public int Version { get; set; }

            // raw_state is the raw states as stored for the resource.  Core does
            // not have access to the schema of prior_version, so it's the
            // provider's responsibility to interpret this value using the
            // appropriate older schema. The raw_state will be the json encoded
            // state, or a legacy flat-mapped format.
            public RawState? RawState { get; set; }
        }

        public class Response
        {
            // new_state is a msgpack-encoded data structure that, when interpreted with
            // the _current_ schema for this resource type, is functionally equivalent to
            // that which was given in prior_state_raw.
            public DynamicValue? UpgradedState { get; set; }

            // diagnostics describes any errors encountered during migration that could not
            // be safely resolved, and warnings about any possibly-risky assumptions made
            // in the upgrade process.
            public IList<Diagnostic>? Diagnostics { get; set; }
        }
    }
}
