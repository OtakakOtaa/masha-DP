using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace CodeBase.Infrastructure
{
    public sealed class SoundProxy : MonoBehaviour
    {
        [Inject] private AudioService _audioService;
        
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _btnClickSFX;


        public void Execute()
        {
            _audioService.PlayEffect(_btnClickSFX);
        }
    }
}