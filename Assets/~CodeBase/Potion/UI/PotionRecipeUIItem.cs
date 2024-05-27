using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionRecipeUIItem : MonoBehaviour
    {
        [SerializeField] private Image _recordImage;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private int _heightOffset = -23;
        
        
        
        public void Init(Sprite entireRow)
        {
            _recordImage.sprite = entireRow;
            _rectTransform.sizeDelta = new Vector2(entireRow.rect.width, entireRow.rect.height - _heightOffset);
        }
        
        
#region OLD
        
        [Obsolete]
        [SerializeField] private Transform _leftContainer;
        
        [Obsolete]
        [SerializeField] private PotionCraftPartUIItem _itemPrefab;
        
        [Obsolete]
        private readonly List<PotionCraftPartUIItem> _parts = new();
        
        [Obsolete]
        public void Init((Sprite sprite, Color color, string mes)[] data)
        {
            for (var i = 0; i < data.Length; i++)
            {
                _parts[i].gameObject.SetActive(true);
        
                if (data[i].mes != string.Empty)
                {
                    _parts[i].InitAsText(data[i].mes);
                    continue;
                }
                
                _parts[i].InitAsImage(data[i].sprite, data[i].color);
            }
        }

        [Obsolete]
        private void UpdateLeftContainer(int referencedSize)
        {
            var newItemsForInstanceCount = referencedSize - _parts.Count;
        
            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_itemPrefab, _leftContainer.transform);
                newItem.transform.localScale = Vector3.one;
                _parts.Add(newItem);
            }
        
            for (var i = _parts.Count - 1; i >= _parts.Count + newItemsForInstanceCount; i--)
            {
                _parts[i].gameObject.SetActive(false);
            }
        }
#endregion

    }
}