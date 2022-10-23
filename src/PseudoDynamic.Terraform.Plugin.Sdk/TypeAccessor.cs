using System.Reflection;
using Concurrent.FastReflection.NetStandard;

namespace PseudoDynamic.Terraform.Plugin
{
    internal class TypeAccessor
    {
        private const BindingFlags _publicInstance = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags _privateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        public Type Type { get; }

        private Dictionary<string, MethodCaller<object, object>> _methodByName = new();
        private readonly Dictionary<int, ConstructorInfo> _constructorByParametersCount = new();

        public TypeAccessor(Type type) =>
            Type = type ?? throw new ArgumentNullException(nameof(type));

        public MethodCaller<object, object> GetMethod(string methodName)
        {
            if (_methodByName.TryGetValue(methodName, out var methodDelegate)) {
                return methodDelegate;
            }

            var method = Type.GetMethod(methodName) ?? throw new InvalidOperationException($"Method {methodName} not found");
            methodDelegate = method.DelegateForCall();
            _methodByName[methodName] = methodDelegate;
            return methodDelegate;
        }

        public ConstructorInfo GetConstructor(int parametersCount, BindingFlags bindingFlags)
        {
            if (_constructorByParametersCount.TryGetValue(parametersCount, out var cachedConstructor)) {
                return cachedConstructor;
            }

            var constructor = Type.GetConstructors(bindingFlags).Single(x => x.GetParameters().Length == parametersCount);
            _constructorByParametersCount[parametersCount] = constructor;
            return constructor;
        }



        public ConstructorInfo GetPublicInstanceConstructor(int parametersCount) =>
            GetConstructor(parametersCount, _publicInstance);

        public ConstructorInfo GetPrivateInstanceConstructor(int parametersCount) =>
            GetConstructor(parametersCount, _privateInstance);

        public object CreateInstance(Func<TypeAccessor, Func<int, ConstructorInfo>> getConstructorInfo, params object?[] arguments) =>
            getConstructorInfo(this)(arguments.Length).Invoke(arguments);
    }
}
