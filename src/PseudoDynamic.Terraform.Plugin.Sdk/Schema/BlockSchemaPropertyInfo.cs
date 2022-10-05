using System.Diagnostics.CodeAnalysis;
using PseudoDynamic.Terraform.Plugin.Types;

namespace PseudoDynamic.Terraform.Plugin.Schema
{
    internal class BlockSchemaPropertyInfo
    {
        /// <summary>
        /// The original property type.
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// The Terraform type of the property.
        /// </summary>
        public TerraformType TerraformType { get; }

        ///// <summary>
        ///// The Terraform type "between" <see cref="PropertyType"/> and <see cref="UnwrappedType"/>.
        ///// It can represent Terraform's "list", "set" or "map".
        ///// This is equivalent to the nesting mode of a block.
        ///// </summary>
        //public TerraformType TerraformElementType
        //{
        //    get
        //    {
        //        if (!HasElement)
        //        {
        //            throw new InvalidOperationException();
        //        }

        //        return _terraformBlockRangeType;
        //    }
        //}


        //public Type? UnwrappedElementType { get; }

        /// <summary>
        /// The unwrapped type. If <see cref="IsTerraformValue"/> is <see langword="false"/>,
        /// then <see cref="UnwrappedType"/> is equals <see cref="PropertyType"/>.
        /// </summary>
        public Type UnwrappedType { get; }

        /// <summary>
        /// Indicates whether the unwrapped property type is nullable.
        /// This can be used to determine if the attribute is required or not.
        /// </summary>
        public bool IsNullable { get; }

        /// <summary>
        /// Indicates whether the property is of <see cref="TerraformValue{T}"/>.
        /// </summary>
        public bool IsTerraformValue { get; }

        BlockSchemaPropertyInfo? ElementInfo { get; init; }

        /// <summary>
        /// Indicates that a block is wrapped by Terraform's "list", "set" or "map".
        /// This is equivalent to the nesting mode of a block.
        /// </summary>
        [MemberNotNullWhen(true, nameof(ElementInfo))]
        public bool HasElementInfo => ElementInfo is not null;

        public BlockSchemaPropertyInfo(
            Type propertyType,
            TerraformType terraformType,
            Type unwrappedType,
            bool isNullable,
            bool isTerraformValue)
        {
            PropertyType = propertyType;
            TerraformType = terraformType;
            UnwrappedType = unwrappedType;
            IsNullable = isNullable;
            IsTerraformValue = isTerraformValue;
        }
    }
}
