﻿using System.Collections.Concurrent;
using System.Reflection;
using Concurrent.FastReflection.NetStandard;
using TypeKitchen.Creation;

namespace PseudoDynamic.Terraform.Plugin.Reflection
{
    internal class TypeAccessor
    {
        private const BindingFlags _privateStaticBindings = BindingFlags.Static | BindingFlags.NonPublic;

        private const BindingFlags _publicInstanceBindings = BindingFlags.Instance | BindingFlags.Public;
        private const BindingFlags _privateInstanceBindings = BindingFlags.Instance | BindingFlags.NonPublic;

        public Type Type { get; }

        private readonly ConcurrentDictionary<string, MethodInfo> _methodByName = new();
        private readonly ConcurrentDictionary<string, MethodAccessor> _methodAccessorByName = new();
        private readonly ConcurrentDictionary<string, MethodCaller<object, object>> _methodCallerByName = new();
        private readonly ConcurrentDictionary<int, ConstructorInfo> _constructorByParametersCount = new();
        private readonly ConcurrentDictionary<int, CreateInstance> _instanceActivatorByParametersCount = new();

        public TypeAccessor(Type type) =>
            Type = type ?? throw new ArgumentNullException(nameof(type));

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags)
        {
            if (_methodByName.TryGetValue(methodName, out var method)) {
                return method;
            }

            method = Type.GetMethod(methodName, bindingFlags) ?? throw new InvalidOperationException($"Method {methodName} not found");
            _methodByName[methodName] = method;
            return method;
        }

        public MethodInfo GetPrivateStaticMethod(string methodName) =>
            GetMethod(methodName, _privateStaticBindings);

        public MethodAccessor GetMethodAccessor(Func<TypeAccessor, Func<string, MethodInfo>> getMethod, string methodName)
        {
            if (_methodAccessorByName.TryGetValue(methodName, out var methodAccessor)) {
                return methodAccessor;
            }

            var method = getMethod(this)(methodName);
            methodAccessor = new MethodAccessor(method);
            _methodAccessorByName[methodName] = methodAccessor;
            return methodAccessor;
        }

        public MethodCaller<object, object> GetMethodCaller(string methodName)
        {
            if (_methodCallerByName.TryGetValue(methodName, out var methodDelegate)) {
                return methodDelegate;
            }

            var method = Type.GetMethod(methodName) ?? throw new InvalidOperationException($"Method {methodName} not found");
            methodDelegate = method.DelegateForCall();
            _methodCallerByName[methodName] = methodDelegate;
            return methodDelegate;
        }

        public MethodCaller<object, object> GetMethodCaller(MethodInfo method) =>
            method.DelegateForCall();

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
            GetConstructor(parametersCount, _publicInstanceBindings);

        public ConstructorInfo GetPrivateInstanceConstructor(int parametersCount) =>
            GetConstructor(parametersCount, _privateInstanceBindings);

        public CreateInstance GetConstructorActivator(int parametersCount, BindingFlags bindingFlags)
        {
            if (_instanceActivatorByParametersCount.TryGetValue(parametersCount, out var instanceActivator)) {
                return instanceActivator;
            }

            var constructorInfo = GetConstructor(parametersCount, bindingFlags);
            instanceActivator = Activation.ExpressionWeakTyped(constructorInfo);
            _instanceActivatorByParametersCount[parametersCount] = instanceActivator;
            return instanceActivator;
        }

        public CreateInstance GetPublicInstanceActivator(int parametersCount) =>
            GetConstructorActivator(parametersCount, _publicInstanceBindings);

        public CreateInstance GetPrivateInstanceActivator(int parametersCount) =>
            GetConstructorActivator(parametersCount, _privateInstanceBindings);

        /// <summary>
        /// Creates an instance by invoking directly the cached <see cref="ConstructorInfo"/>.
        /// </summary>
        /// <param name="getConstructorInfo"></param>
        /// <param name="arguments"></param>
        public object CreateInstance(Func<TypeAccessor, Func<int, ConstructorInfo>> getConstructorInfo, params object?[] arguments) =>
            getConstructorInfo(this)(arguments.Length).Invoke(arguments);

        /// <summary>
        /// Creates an instance by invoking a cached compiled expression that represents the constructor.
        /// </summary>
        /// <param name="getInstanceActivator"></param>
        /// <param name="arguments"></param>
        public object CreateInstance(Func<TypeAccessor, Func<int, CreateInstance>> getInstanceActivator, params object?[] arguments) =>
            getInstanceActivator(this)(arguments.Length).Invoke(arguments);
    }
}
