using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _CodeBase.Customers._Data;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Hall
{
    public sealed class DialogueBubble : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageFld;
        [SerializeField] private Button _confirmBtn;
        [SerializeField] private Button _concreteHintBtn;
        [SerializeField] private Canvas _canvas;
        
        
        [SerializeField] private float _textAppearTemp = 1f;
        
        
        private readonly CompositeDisposable _compositeDisposable = new();
        private CancellationTokenSource _messAnimProcess;
        private Order _activeOrder;

        public bool BubbleOpenedFlag { get; private set; }

        
        private void Awake()
        {
            _canvas.worldCamera = Camera.main;
            Deactivate().Forget();
            
            _confirmBtn.OnClickAsObservable()
                .Subscribe(_ => Deactivate().Forget())
                .AddTo(_compositeDisposable);
            
            _concreteHintBtn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _concreteHintBtn.gameObject.SetActive(false);
                    _messAnimProcess?.Cancel();
                    _messAnimProcess = new CancellationTokenSource();
                    FillTextFld(_activeOrder.ConcreteMessage, _messAnimProcess.Token).Forget();
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public async UniTask Activate(Order order)
        {
            _messAnimProcess?.Cancel();
            _messAnimProcess = new CancellationTokenSource();
            _activeOrder = order;

            _messageFld.text = string.Empty;
            gameObject.SetActive(true);
            _concreteHintBtn.gameObject.SetActive(true);
            BubbleOpenedFlag = true;
            
            await FillTextFld(order.Message, _messAnimProcess.Token);
        }

        public async UniTask Deactivate()
        {
            _messAnimProcess?.Cancel();
            gameObject.SetActive(false);
            BubbleOpenedFlag = false;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async UniTask FillTextFld(string mess, CancellationToken cancellationToken)
        {
            const float delayDelta = 0.2f;
            const float spaceSignDelta = 0.5f;

            var textSource = new StringBuilder();
            var delay = _textAppearTemp / (mess.Length + 1);
            
            
            foreach (var @char in mess)
            {
                _messageFld.text = textSource.Append(@char).ToString();
                await UniTask.WaitForSeconds(delay * (@char == ' ' ? spaceSignDelta : delayDelta), cancellationToken: cancellationToken);
            }
        } 
    }
}