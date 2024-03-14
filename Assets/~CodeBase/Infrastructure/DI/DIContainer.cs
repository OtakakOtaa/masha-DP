using VContainer.Unity;

namespace _CodeBase.Infrastructure.DI
{
    public sealed class DiContainer
    {
        public DiContainer(LifetimeScope globalScope)
        {
            GlobalScope = globalScope;
            Scope = globalScope;
        }

        public LifetimeScope GlobalScope { get; }

        public LifetimeScope Scope { get; set; }
    }
}