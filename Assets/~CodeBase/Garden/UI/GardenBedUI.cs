using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenBedUI : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        
        public void UpdateProgressBar(float delta)
        {
            _progressBar.value = delta;
        }
    }
}