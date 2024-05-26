using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionCraftPartUIItem : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Init(Sprite sprite, Color color)
        {
            _image.sprite = sprite;
            _image.color = color;
        }
    }
}