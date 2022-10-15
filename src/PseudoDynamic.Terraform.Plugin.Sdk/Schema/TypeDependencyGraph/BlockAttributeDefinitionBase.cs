namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph
{
    /// <summary>
    /// Defines the base of an attribute or a nested block.
    /// </summary>
    internal abstract record class BlockAttributeDefinitionBase : AttributeDefinition
    {
        /// <summary>
        /// The description of this attribute.
        /// </summary>
        public string Description {
            get => _description;
            init => _description = value ?? string.Empty;
        }

        /// <inheritdoc/>
        public override bool IsRequired {
            get => _isRequired;
            init {
                _isRequired = value;
                _isOptional = !value;

                if (value) {
                    // Disable the attribute being computed when it is required
                    _isComputed = false;
                }
            }
        }

        /// <inheritdoc/>
        public override bool IsOptional {
            get => _isOptional;

            init {
                _isOptional = value;

                if (!_isComputed) {
                    // Prevents the change of being required or not when it is computed
                    _isRequired = !value;
                }
            }
        }

        /// <summary>
        /// The value this attribute describes will be computed.
        /// </summary>
        public bool IsComputed {
            get => _isComputed;

            init {
                _isComputed = value;

                if (value) {
                    // Disable the attribute being required when it is computed
                    _isRequired = false;
                }
            }
        }

        /// <summary>
        /// Marks this attribute as sensitive.
        /// </summary>
        public bool IsSensitive { get; init; }

        /// <summary>
        /// Kind of description.
        /// </summary>
        public DescriptionKind DescriptionKind { get; init; }

        /// <summary>
        /// Marks this attribute as deprecated.
        /// </summary>
        public bool IsDeprecated { get; init; }

        private string _description = string.Empty;
        private bool _isOptional;
        private bool _isRequired = DefaultIsRequired;
        private bool _isComputed;

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BlockAttributeDefinitionBase(string name, ValueDefinition value)
            : base(name, value)
        {
        }
    }
}
