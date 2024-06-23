using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Potion.UI
{
    public sealed class PotionCraftPartUIItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
         
        
        public void InitAsImage(Sprite sprite, Color color)
        {
            _image.enabled = true;
            _text.enabled = false;
            _image.sprite = sprite;
            _image.preserveAspect = true;

            
            _image.color = color;
        }
        
        public void InitAsText(string text)
        {
            _text.text = text;
            _image.enabled = false;
        }
    }
}