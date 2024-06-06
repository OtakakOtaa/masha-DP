using UnityEngine;
using UnityEngine.UI;

namespace _CodeBase.Garden.UI
{
    public sealed class GardenBedUI : MonoBehaviour
    {
        [SerializeField] private Slider _progressBar;
        [SerializeField] private GameObject _careRequiredTag;
        
        
        public void UpdateProgressBar(float delta)
        {
            _progressBar.value = delta;
        }

        public void ShowProgressBar()
        {
            _progressBar.gameObject.SetActive(true);
        }
        
        public void ShowCareRequiredTag()
        {
            _careRequiredTag.gameObject.SetActive(true);
        }

        
        public void HideCareRequiredTag()
        {
            _careRequiredTag.gameObject.SetActive(false);
        }
        
        public void HideProgressBar()
        {
            _progressBar.gameObject.SetActive(false);
        }

        
        public void HideAll()
        {
            HideProgressBar();
            HideCareRequiredTag();
        }
    }
}