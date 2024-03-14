
using JetBrains.Annotations;

namespace _CodeBase.MainGameplay
{
    public sealed class GameplayService
    {
        [NotNull] public static GameplayService Instance { get; private set; }
        
        [CanBeNull] public GameplayState gameplayState;

        public GameplayService() => Instance = this;
    }
}