using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Garden
{
    public sealed class GardenBedUI : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;
        
        public void UpdateProgressBar(float delta)
        {
            _progressBar.fillAmount = delta;
        }
    }
}