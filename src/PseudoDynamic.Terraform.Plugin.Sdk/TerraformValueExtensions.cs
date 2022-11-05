namespace PseudoDynamic.Terraform.Plugin
{
    /// <summary>
    /// Contains extension methods for <see cref="ITerraformValue{T}"/>.
    /// </summary>
    public static class TerraformValueExtensions
    {
        /// <summary>
        /// The inversion of <see cref="IsNullOrUnknown{T}(ITerraformValue{T}?, out T)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terraformValue"></param>
        /// <param name="value"></param>
        /// <returns><see langword="true"/> if not null and not unknown.</returns>
        public static bool TryGetValue<T>(this TerraformValue<T>? terraformValue, [MaybeNullWhen(false)] out T value)
        {
            if (terraformValue is null) {
                value = default;
                return false;
            }

            value = terraformValue.Value;
            // Because the interface may be implemented by user, we check both explicitly
            return !terraformValue.IsNull && !terraformValue.IsUnknown;
        }

        /// <summary>
        /// Checks whether <paramref name="terraformValue"/> is a Terraform null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terraformValue"></param>
        /// <param name="value"></param>
        /// <returns><see langword="true"/> if null.</returns>
        public static bool IsNull<T>(this ITerraformValue<T>? terraformValue, [MaybeNullWhen(true)] out T value)
        {
            if (terraformValue is null) {
                value = default;
                return true;
            }

            value = terraformValue.Value;
            // Because the interface may be implemented by user, we check both explicitly
            return terraformValue.IsNull || terraformValue.IsUnknown;
        }

        /// <summary>
        /// The inversion of <see cref="TryGetValue{T}(TerraformValue{T}?, out T)"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terraformValue"></param>
        /// <param name="value"></param>
        /// <returns><see langword="true"/> if null or unknown.</returns>
        public static bool IsNullOrUnknown<T>(this ITerraformValue<T>? terraformValue, [MaybeNullWhen(true)] out T value)
        {
            if (terraformValue is null) {
                value = default;
                return true;
            }

            value = terraformValue.Value;
            return terraformValue.IsNull || terraformValue.IsUnknown;
        }

        /// <summary>
        /// Checks whether <paramref name="terraformValue"/> is a Terraform unknown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terraformValue"></param>
        /// <returns><see langword="true"/> if unknown.</returns>
        public static bool IsUnknown<T>(this ITerraformValue<T>? terraformValue) =>
            terraformValue is null ? false : terraformValue.IsUnknown;
    }
}
