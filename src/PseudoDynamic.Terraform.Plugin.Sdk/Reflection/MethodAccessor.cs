using System.Reflection;

namespace PseudoDynamic.Terraform.Plugin.Reflection
{
    internal class MethodAccessor
    {
        public MethodInfo Method { get; }

        private readonly Dictionary<Type, MethodInfo> _methodByOneTypeArgument = new();

        public MethodAccessor(MethodInfo method) => Method = method;

        public MethodInfo MakeGenericMethod(Type typeArgument)
        {
            if (_methodByOneTypeArgument.TryGetValue(typeArgument, out MethodInfo? method)) {
                return method;
            }

            method = Method.MakeGenericMethod(typeArgument);
            _methodByOneTypeArgument[typeArgument] = method;
            return method;
        }
    }
}
