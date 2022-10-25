using System.Reflection;
using PseudoDynamic.Terraform.Plugin.Conventions;

namespace PseudoDynamic.Terraform.Plugin.Schema.Conventions
{
    internal class AttributeNameConvention : IAttributeNameConvention
    {
        private static string GetAttributeName(PropertyInfo attribute)
        {
            var nameAttribute = attribute.GetCustomAttribute<NameAttribute>();

            if (nameAttribute is not null) {
                return nameAttribute.Name;
            }

            return attribute.Name;
        }

        private INameConvention _nameConvention;

        /// <summary>
        /// Creates an convention to format an attribute properly.
        /// </summary>
        /// <param name="nameConvention"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AttributeNameConvention(INameConvention nameConvention) =>
            _nameConvention = nameConvention ?? throw new ArgumentNullException(nameof(nameConvention));

        /// <inheritdoc/>
        public string Format(PropertyInfo attribute) => _nameConvention.Format(GetAttributeName(attribute));
    }
}
