namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Annotates a class for being used as Terraform (nested) block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class BlockAttribute : BlockLikeAttribute
    {
        private int _version = -1;

        public BlockAttribute() : base(TerraformTypeConstraint.Block)
        {
        }

        public BlockAttribute(int version)
            : this()
        {
            if (version < 0) {
                throw new ArgumentOutOfRangeException("Version can not be negative");
            }

            _version = version;
        }

        internal int? GetVersion()
        {
            if (_version == -1) {
                return default;
            }

            return _version;
        }
    }
}
