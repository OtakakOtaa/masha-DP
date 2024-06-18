using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Infrastructure
{
    public sealed class GifAnimation : MonoBehaviour
    {
        public string Filename;
        [SerializeField] private bool _loopFlag = false;
        [SerializeField] private bool _isImage = false;
        
        
        [PreviewField]
        [SerializeField] private List<Sprite> _mFrames = new();
        [SerializeField] private List<float> _mFrameDelay = new();

        private float[] _mFrameDelayRatio;
        private int _mCurFrame = 0;
        private float _mTime = 0.0f;
        private Sprite _originSprite;
        private Color _originColor;
        private SpriteRenderer _spriteRenderer;
        private Image _image; 
        private IDisposable _updateSub;
        private UniTaskCompletionSource _oneLoopPlayed;

#if UNITY_EDITOR
        [Button("Bake")]
        private void Bake()
        {
            _mFrames.Clear();
            _mFrameDelay.Clear();
            if (string.IsNullOrWhiteSpace(Filename)) return;

            var path = Path.Combine(Application.streamingAssetsPath, Filename);

            using (var decoder = new MG.GIF.Decoder(File.ReadAllBytes(path)))
            {
                var img = decoder.NextImage();

                while (img != null)
                {
                    var tex = img.CreateTexture();
                    _mFrames.Add(
                        Instantiate(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f, 100)));
                    _mFrameDelay.Add(img.Delay / 1000.0f);
                    img = decoder.NextImage();
                }
            }

            _image = GetComponent<Image>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
#endif

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _image = GetComponent<Image>();
            
            if (_loopFlag)
            {
                SetSprite(_mFrames[0]);
                GameService.GameUpdate.Subscribe(_ => UpdateGif()).AddTo(destroyCancellationToken);
            }
        }

        
        public async UniTask Play(Color? color = null, float? duration = null)
        {
            _updateSub?.Dispose();
            _oneLoopPlayed?.TrySetCanceled();
            _oneLoopPlayed = null;

            _originColor = _isImage ? _image.color : _spriteRenderer.color;
            if (color != null) SetColor(color.Value);
            
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
            _image ??= GetComponent<Image>();

            _originSprite = _isImage ? _image.sprite : _spriteRenderer.sprite;
            ResetDelta();

            if (duration != null) SetDuration(duration.Value);
            
            _updateSub = GameService.GameUpdate.Subscribe(_ => UpdateGif());
        
            _oneLoopPlayed = new UniTaskCompletionSource();
            await _oneLoopPlayed.Task;

            SetSprite(_originSprite);
            SetColor(_originColor);
            
            _updateSub?.Dispose();
            _updateSub = null;
        }

        public void Kill()
        {
            _updateSub?.Dispose();
            _oneLoopPlayed?.TrySetCanceled();
            _oneLoopPlayed = null;
            _updateSub?.Dispose();
            _updateSub = null;
            
            ResetDelta();
        }
        
        public void SetColor(Color color)
        {
            if (_isImage)
            {
                _image.color = color;
            }
            else
            {
                _spriteRenderer.color = color;
            }
        }

        private void UpdateGif()
        {
            if (_mFrames == null) return;

            _mTime += Time.deltaTime;

            if (_mTime >= _mFrameDelay[_mCurFrame])
            {
                _mCurFrame = (_mCurFrame + 1) % _mFrames.Count;
                _mTime = 0.0f;

                SetSprite(_mFrames[_mCurFrame]);
                
                if (_mCurFrame + 1 == _mFrameDelay.Count && _oneLoopPlayed != null)
                {
                    _oneLoopPlayed.TrySetResult();
                    _oneLoopPlayed = null;
                }
            }
        }

        private void ResetDelta()
        {
            _mCurFrame = 0;
            _mTime = 0.0f;
            SetSprite(_mFrames[_mCurFrame]);
        }

        private void SetSprite(Sprite sprite)
        {
            if (_isImage)
            {
                _image.sprite = sprite;
            }
            else
            {
                _spriteRenderer.sprite = sprite;
            }
        }

        private void SetDuration(float newDuration)
        {
            _mFrameDelayRatio ??= new float[_mFrameDelay.Count];
            
            var currentDuration = 0f;
            for (var i = 0; i < _mFrameDelay.Count; i++)
            {
                currentDuration += _mFrameDelay[i];
            }
            
            for (var i = 0; i < _mFrameDelay.Count; i++)
            {
                _mFrameDelayRatio[i] = _mFrameDelay[i] / currentDuration;
                _mFrameDelay[i] = _mFrameDelayRatio[i] * newDuration;
            }
            
        }
    }
}