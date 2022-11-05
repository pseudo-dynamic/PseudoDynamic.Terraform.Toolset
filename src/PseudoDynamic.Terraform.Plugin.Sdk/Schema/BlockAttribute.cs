namespace PseudoDynamic.Terraform.Plugin.Schema
{
    /// <summary>
    /// Annotates a class for being used as Terraform block.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BlockAttribute : ComplexAttribute
    {
        private int _version = -1;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
        public BlockAttribute() : base(TerraformTypeConstraint.Block)
        {
        }

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="version">The schema version. Can get be overriden by the version of <see cref="NestedBlockAttribute"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [SuppressMessage("ApiDesign", "RS0022:Constructor make noninheritable base class inheritable", Justification = "Prevents inheriting base")]
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
