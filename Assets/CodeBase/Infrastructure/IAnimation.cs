using DG.Tweening;

namespace _CodeBase.Infrastructure.UI
{
    public interface IAnimation
    {
        void Play(float? duration = null);

        Tween Tween { get; }
    }
}