using System.Collections;
using System.Collections.Generic;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

public class AeTest : MonoBehaviour
{

    [SerializeField] private AudioService _audioService;
    
    [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
    [SerializeField] private string _sfx;


    [Button("GG")]
    public void GG()
    {
        _audioService.PlayEffect(_sfx);
    }
}

    
