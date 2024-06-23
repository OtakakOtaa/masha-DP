using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace CodeBase.Audio
{
    [Serializable] public sealed class Sound
    {
        [SerializeField] private string _soundName;
        [SerializeField] private AudioClip _soundClip;
        [SerializeField] private AudioMixerGroup _mixerGroup;
        [SerializeField] bool _doRandomVolume;
        [ShowIf("DoRandomVolume")]
        [MinMaxSlider(0, 1)]
        [SerializeField] private Vector2 _volumeRange = new Vector2(0.4f, 0.6f);
        [HideIf("DoRandomVolume")]
        [SerializeField] [Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] bool _doRandomPitch;
        [ShowIf("DoRandomPitch")]
        [MinMaxSlider(0, 2)]
        [SerializeField] private Vector2 _pitchRange = new Vector2(-1f, 1f);
        [HideIf("DoRandomPitch")]
        [SerializeField] [Range(-3f, 3f)] private float _pitch = 1f;
        [SerializeField] bool _doRandomPriority;
        [ShowIf("DoRandomPriority")]
        [MinMaxSlider(0, 256)]
        [SerializeField] private Vector2Int _priorityRange = new Vector2Int(100, 156);
        [HideIf("DoRandomPriority")]
        [SerializeField][Range(0, 256)] private int _priority = 128;
        [SerializeField] private float _dopplerLevel = 1f;
        [SerializeField] private bool _isLooped;
        [SerializeField] private float _fadeDuration;
        


        public string SoundName => _soundName;
        public AudioClip SoundClip => _soundClip;
        public AudioMixerGroup MixerGroup => _mixerGroup;
        public Vector2 VolumeRange => _volumeRange;
        public Vector2 PitchRange => _pitchRange;
        public Vector2Int PriorityRange => _priorityRange;
        public int Priority => _priority;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public float DopplerLevel => _dopplerLevel;
        public bool DoRandomVolume => _doRandomVolume;
        public bool DoRandomPitch => _doRandomPitch;
        public bool DoRandomPriority => _doRandomPriority;
        public bool IsLooped => _isLooped;
        public float FadeDuration => _fadeDuration;
    }
}