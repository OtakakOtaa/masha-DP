using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _CodeBase.Shop
{
    public class ShopUIItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _headerTMP;
        [FormerlySerializedAs("_coastTMP")] [SerializeField] private TMP_Text _costTMP;
        [SerializeField] private Image _image;
        [SerializeField] private Button _buyBtn;
        
        
        public readonly ReactiveCommand BuyRequestEvent = new();
        
        private void Awake()
        {
            _buyBtn.OnClickAsObservable().Subscribe(_ => BuyRequestEvent?.Execute()).AddTo(destroyCancellationToken);
        }

        public void Init(Sprite sprite, string titleName, int cost, Vector2 rect)
        {
            _image.rectTransform.sizeDelta = rect;
            _image.preserveAspect = true;
            _image.sprite = sprite;
            _headerTMP.text = string.Format("{0:C}", titleName);
            _costTMP.text = cost.ToString();
        }
    }
}