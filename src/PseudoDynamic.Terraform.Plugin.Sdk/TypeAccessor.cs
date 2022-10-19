using Concurrent.FastReflection.NetStandard;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class TypeAccessor
    {
        private readonly Type _type;
        private Dictionary<string, MethodCaller<object, object>> _methods = new();

        public TypeAccessor(Type type) =>
            _type = type ?? throw new ArgumentNullException(nameof(type));

        public MethodCaller<object, object> GetMethod(string methodName)
        {
            if (_methods.TryGetValue(methodName, out var methodDelegate)) {
                return methodDelegate;
            }

            var method = _type.GetMethod(methodName) ?? throw new InvalidOperationException($"Method {methodName} not found");
            methodDelegate = method.DelegateForCall();
            _methods[methodName] = methodDelegate;
            return methodDelegate;
        }
    }
}
