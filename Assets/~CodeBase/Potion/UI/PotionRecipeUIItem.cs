using System.Collections.Generic;
using UnityEngine;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionRecipeUIItem : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private PotionCraftPartUIItem _itemPrefab;
        
        
        private readonly List<PotionCraftPartUIItem> _parts = new();
        
        public void Init((Sprite sprite, Color color)[] data)
        {
            var newItemsForInstanceCount = data.Length - _parts.Count;

            for (var i = 0; i < newItemsForInstanceCount; i++)
            {
                var newItem = Instantiate(_itemPrefab, _container.transform);
                newItem.transform.localScale = Vector3.one;
                _parts.Add(newItem);
            }

            for (var i = _parts.Count - 1; i >= _parts.Count + newItemsForInstanceCount; i--)
            {
                _parts[i].gameObject.SetActive(false);
            }

            for (var i = 0; i < data.Length; i++)
            {
                _parts[i].gameObject.SetActive(true);
                _parts[i].Init(data[i].sprite, data[i].color);
            }
        }
    }
}