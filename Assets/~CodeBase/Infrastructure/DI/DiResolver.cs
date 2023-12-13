using UnityEngine;
using VContainer.Unity;

namespace _CodeBase.Infrastructure.DI
{
    public sealed class DiResolver
    {
        public LifetimeScope GlobalScope { get; }

        public DiResolver(LifetimeScope globalScope)
        {
            GlobalScope = globalScope;
        }

        
        public LifetimeScope GetSceneScopeOrGlobal()
            => Object.FindObjectOfType<LifetimeScope>() ?? GlobalScope;
    }
}