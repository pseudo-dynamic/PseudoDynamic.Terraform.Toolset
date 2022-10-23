using System.Reflection;
using Concurrent.FastReflection.NetStandard;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class TypeAccessor
    {
        private const BindingFlags _publicInstance = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags _privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        private readonly Type _type;
        private Dictionary<string, MethodCaller<object, object>> _methodByName = new();
        private readonly Dictionary<int, ConstructorInfo> _constructorByParametersCount = new();

        public TypeAccessor(Type type) =>
            _type = type ?? throw new ArgumentNullException(nameof(type));

        public MethodCaller<object, object> GetMethod(string methodName)
        {
            if (_methodByName.TryGetValue(methodName, out var methodDelegate)) {
                return methodDelegate;
            }

            var method = _type.GetMethod(methodName) ?? throw new InvalidOperationException($"Method {methodName} not found");
            methodDelegate = method.DelegateForCall();
            _methodByName[methodName] = methodDelegate;
            return methodDelegate;
        }

        public ConstructorInfo GetConstructor(int parametersCount, BindingFlags bindingFlags)
        {
            if (_constructorByParametersCount.TryGetValue(parametersCount, out var cachedConstructor)) {
                return cachedConstructor;
            }

            var constructor = _type.GetConstructors(bindingFlags).Single(x => x.GetParameters().Length == parametersCount);
            _constructorByParametersCount[parametersCount] = constructor;
            return constructor;
        }



        public ConstructorInfo GetPublicInstanceConstructor(int parametersCount) =>
            GetConstructor(parametersCount, _publicInstance);

        public ConstructorInfo GetPrivateInstanceConstructor(int parametersCount) =>
            GetConstructor(parametersCount, _privateInstance);

        public object InvokeConstructor(Func<TypeAccessor, Func<int, ConstructorInfo>> getConstructorInfo, params object?[] arguments) =>
            getConstructorInfo(this)(arguments.Length).Invoke(arguments);
    }
}
