using System.Text;
using System.Threading;
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
        
        
        private readonly CompositeDisposable _compositeDisposable = new();
        private CancellationTokenSource _messAnimProcess;
        private Order _activeOrder;

        public bool BubbleOpenedFlag { get; private set; }

        
        private void Awake()
        {
            _canvas.worldCamera = Camera.main;
            Deactivate();
            
            _confirmBtn.OnClickAsObservable()
                .Subscribe(_ => Deactivate())
                .AddTo(_compositeDisposable);
            
            _concreteHintBtn.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _concreteHintBtn.gameObject.SetActive(false);
                    _messAnimProcess?.Cancel();
                    _messAnimProcess = new CancellationTokenSource();
                    ExecuteMessFill(_activeOrder.ConcreteMessage, _messAnimProcess.Token).Forget();
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
            Activate(showButtons: true);
            _activeOrder = order;
            await ExecuteMessFill(order.Message, _messAnimProcess.Token);
        }

        public void Activate(bool showButtons = true)
        {
            _messAnimProcess?.Cancel();
            _messAnimProcess = new CancellationTokenSource();
            
            _messageFld.text = string.Empty;
            gameObject.SetActive(true);
            _concreteHintBtn.gameObject.SetActive(showButtons);
            _confirmBtn.gameObject.SetActive(showButtons);
            BubbleOpenedFlag = true;
        }
        
        public void Deactivate()
        {
            _messAnimProcess?.Cancel();
            gameObject.SetActive(false);
            BubbleOpenedFlag = false;
        }

        public async UniTask ExecuteMessFill(string mess, CancellationToken cancellationToken)
        {
            const float delayDelta = 0.2f;
            const float spaceSignDelta = 0.5f;

            var textSource = new StringBuilder();


            for (var i = 0; i < mess.Length; i++)
            {
                var @char = mess[i];
                var currentDelay = GameplayConfig.Instance.GetTextAppearsTemp(i / (float)(mess.Length - 1)) / (float)(mess.Length + 1);
                _messageFld.text = textSource.Append(@char).ToString();
                await UniTask.WaitForSeconds(currentDelay * (@char == ' ' ? spaceSignDelta : delayDelta), cancellationToken: cancellationToken);
            }
        } 
    }
}