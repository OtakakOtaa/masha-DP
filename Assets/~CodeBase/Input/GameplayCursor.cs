using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace _CodeBase.Input
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class GameplayCursor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private CompositeDisposable _compositeDisposable;

        [CanBeNull] public object Item { get; private set; }


        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }

        
        public void AttachItem(object item, Sprite sprite, Action update = default)
        {
            GameService.GameUpdate.Subscribe(_ => update.Invoke()).AddTo(_compositeDisposable);
            Item = item;
            _spriteRenderer.sprite = sprite;
        }
        
        public void DetachItem()
        {
            _compositeDisposable?.Dispose();
            Item = null;
            _spriteRenderer.sprite = null;
        }
    }
}