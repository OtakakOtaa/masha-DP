using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _CodeBase.Infrastructure.DI
{
    public static class VContainerExtensions
    {
        #region CreateInstanceFromDI

        public static TResult ResolveFromContainer<TResult>(this LifetimeScope scope) where TResult : class
        {
            if (typeof(TResult).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var mono = UnityEngine.Object.FindObjectOfType(typeof(TResult)) as TResult;
                scope.Container.Inject(mono);
                return mono;
            }
            
            var constructors = typeof(TResult).GetConstructors();
            ConstructorInfo injectionCtor = null;

            var isOneConstructor = constructors.Length is 1;
            injectionCtor = isOneConstructor is false
                ? FindInjectionCtorOrThrow(constructors)
                : constructors.First();

            return CreateInstanceOrThrow<TResult>(injectionCtor, scope);
        }

        private static void Inject<TComponent>(TComponent component, LifetimeScope scope)
        {
            component.GetType().GetFields()
                .Where(HasInjectAttribute)
                .ForEach(f =>
                {
                    if (TryResolve(scope, f.FieldType, out var service))
                        f.SetValue(component, service);
                    else
                        Debug.LogWarning($"{component.GetType().Name} has no register dependency - fall resolve");
                });
        }
        
        private static ConstructorInfo FindInjectionCtorOrThrow(IEnumerable<ConstructorInfo> ctors)
        {
            var injectionCtor = ctors.FirstOrDefault(HasInjectAttribute);
            var dontHaveDiConstructor = injectionCtor == default;

            if (dontHaveDiConstructor)
                throw SeveralConstructorsWithoutInjectEx;
            return injectionCtor;
        }

        private static TResult CreateInstanceOrThrow<TResult>(ConstructorInfo ctor, LifetimeScope resolver)
        {
            var parameters = ctor.GetParameters();
            var parametersValue = new object[parameters.Length];

            for (var i = 0; i < parametersValue.Length; i++)
            {
                var isServiceExist = TryResolve(resolver, parameters[i].ParameterType, out var param);
                if (isServiceExist is false) throw NonUniformDependenciesEx;
                parametersValue[i] = param;
            }

            return (TResult)ctor.Invoke(parametersValue);
        }

        private static bool TryResolve(LifetimeScope resolver, Type resolveType, out object service)
        {
            while (true)
            {
                service = default;
                try
                {
                    service = resolver.Container.Resolve(resolveType);
                    return true;
                }
                catch (Exception notFoundDependencyEx)
                {
                    if (resolver.IsRoot) return false;
                    resolver = resolver.Parent;
                }
            }
        }

        private static bool HasInjectAttribute(ICustomAttributeProvider ctor)
            => ctor.GetCustomAttributes(false).FirstOrDefault(a => a is InjectAttribute) != default;

        #endregion
        
        private static readonly Exception SeveralConstructorsWithoutInjectEx
            = new("There are several constructors that were not explicitly defined with Inject!");

        private static readonly Exception NonUniformDependenciesEx
            = new("Dependencies not found during instantiation");
    }
}