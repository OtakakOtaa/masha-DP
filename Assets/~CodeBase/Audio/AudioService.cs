using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Audio
{
    public sealed class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioServiceSettings _serviceSettings;

        [SerializeField] private AudioSource _firstAmbienceSource;
        [SerializeField] private AudioSource _secondAmbienceSource;
        [SerializeField] private AudioSource[] _generalSources;

        private Queue<SoundQueueElement> _ambienceQueue;
        private readonly Dictionary<string, AudioSource> _extraAmbiences = new();
        private Dictionary<string, Sequence> _extraAmbiencesFadeDownBrowser = new();
        private readonly Dictionary<string, Sequence> _extraAmbiencesFadeUpBrowser = new();
        
        private Sequence _blendingAmbienceSequence;

        private Sound _cachedAmbienceSound;
        private float _cachedAmbiencePitch;
        private bool _areEffectsSoundsEnabled = true;
        private Sequence _hideAmbientFadeTween;

        public static AudioService Instance {get; private set; }

        public float CachedAmbiencePitch => _cachedAmbiencePitch;


        public void Start()
        {
            Instance = this;
            _ambienceQueue = new Queue<SoundQueueElement>();
        }
        
        
        public void ChangeAmbience(string soundName, float blendTime = 0f, bool doOverlap = false)
        {
            _cachedAmbienceSound = _serviceSettings.GetAmbienceSoundByName(soundName);
            if (_cachedAmbienceSound == null)
            {
                _firstAmbienceSource.Stop();
            }
            else if (_cachedAmbienceSound.SoundClip != _firstAmbienceSource.clip || _firstAmbienceSource.clip == null)
            {
                _ambienceQueue.Enqueue(new SoundQueueElement(_cachedAmbienceSound, blendTime, doOverlap));
                PlayNextAmbienceList();
            }
        }
        

        public void StopAmbience()
        {
            _firstAmbienceSource.Stop();
            _secondAmbienceSource.Stop();
        }

        public void ResetAmbience()
        {
            _hideAmbientFadeTween?.Kill();
            _firstAmbienceSource.clip = null;
            _firstAmbienceSource.Play();
            _secondAmbienceSource.clip = null;
            _secondAmbienceSource.Stop();
        }

        public void HideAmbience(float fadeDuration = 0f)
        {
            _hideAmbientFadeTween = DOTween.Sequence()
                    .Append(_firstAmbienceSource.DOFade(0f, fadeDuration))
                    .Join(_secondAmbienceSource.DOFade(0f, fadeDuration))
                    .OnKill(() =>
                    {
                        _firstAmbienceSource.pitch = 0f;
                        _secondAmbienceSource.pitch = 0f;
                    });
        }

        public void PlayExtraAmbience(string key)
        {
            if (_extraAmbiences.ContainsKey(key) is false)
            {
                AudioSource freeSource = null;
                for (var i = 0; i < _generalSources.Length; i++)
                {
                    if (_generalSources[i].isPlaying is true) continue;
                
                    freeSource = _generalSources[i];
                    break;
                }

                if (freeSource == null)
                {
                    Debug.LogWarning("don't have enough sources");
                    return;
                }
                
                _extraAmbiences[key] = freeSource;
            }
            
            var sound = _serviceSettings.GetAmbienceSoundByName(key);
            if (sound == null)
            {
                Debug.LogWarning($"don't have {key}: sound");
                return;
            }
            
            SetUpSourceSettings(_extraAmbiences[key], sound);
            _extraAmbiences[key].Play();
        }
        
        public void HideExtraAmbience(string key)
        {
            if (_extraAmbiences.ContainsKey(key) is false) return;
            if (_extraAmbiencesFadeUpBrowser.ContainsKey(key) && _extraAmbiencesFadeUpBrowser[key].IsPlaying())
            {
                _extraAmbiencesFadeUpBrowser[key].Kill();
            }

            _extraAmbiencesFadeDownBrowser[key] = DOTween.Sequence()
                .Append(_extraAmbiences[key].DOFade(0f, _serviceSettings.GetAmbienceSoundByName(key).FadeDuration))
                .OnComplete(() => _extraAmbiences[key].pitch = 0f);
        }
        
        public void LaunchExtraAmbience(string key)
        {
            if (_extraAmbiences.ContainsKey(key) is false) return;
            if (_extraAmbiencesFadeDownBrowser.ContainsKey(key) && _extraAmbiencesFadeDownBrowser[key].IsPlaying())
            {
                _extraAmbiencesFadeDownBrowser[key].Kill();
            }
            
            _extraAmbiencesFadeUpBrowser[key] = DOTween.Sequence()
                .Append(_extraAmbiences[key].DOFade(1f, _serviceSettings.GetAmbienceSoundByName(key).FadeDuration))
                .OnComplete(() => _extraAmbiences[key].pitch = 1f);
        }
        
        public void ClearExtraAmbience(string key)
        {
            _extraAmbiencesFadeUpBrowser[key]?.Complete();
            _extraAmbiencesFadeDownBrowser[key]?.Complete();
            _extraAmbiencesFadeUpBrowser.Remove(key);
            _extraAmbiences.Remove(key);
        }
        
        public void PlayEffect(string soundName)
        {
            PlayEffectSound(_generalSources, _serviceSettings.GetEffectSoundByName(soundName));
        }

        public void ContinueAmbience()
        {
            _firstAmbienceSource.pitch = _cachedAmbiencePitch;
            _secondAmbienceSource.pitch = _cachedAmbiencePitch;
        }

        public void PlayEffectSoundInLeftEar(string soundName)
        {
            PlayEffectSound(_generalSources, _serviceSettings.GetEffectSoundByName(soundName));
        }

        public void PlayEffectSoundInRightEar(string soundName)
        {
            PlayEffectSound(_generalSources, _serviceSettings.GetEffectSoundByName(soundName));
        }

        public void DisableEffectsSounds()
        {
            _areEffectsSoundsEnabled = false;
        }

        public void EnableEffectsSounds()
        {
            _areEffectsSoundsEnabled = true;
        }

        public void StopEffectSound(string soundName)
        {
            var sound = _serviceSettings.GetEffectSoundByName(soundName);
            foreach (var source in _generalSources)
            {
                if (source.isPlaying && source.loop && source.clip == sound.SoundClip)
                {
                    source.Stop();
                }
            }
        }

        
        private void PlayNextAmbienceList()
        {
            if (_blendingAmbienceSequence == null || (!_blendingAmbienceSequence.IsPlaying() && _ambienceQueue.Count > 0))
            {
                BlendAmbienceFromQueue();
            }
        }
        
        private void BlendAmbienceFromQueue()
        {
            _hideAmbientFadeTween?.Kill();
            
            var nextAmbience = _ambienceQueue.Peek();
            _secondAmbienceSource.clip = nextAmbience.Sound.SoundClip;
            _cachedAmbiencePitch = nextAmbience.Sound.DoRandomPitch
                ? Random.Range(nextAmbience.Sound.PitchRange.x, nextAmbience.Sound.PitchRange.y)
                : nextAmbience.Sound.Pitch;
            _secondAmbienceSource.pitch = _cachedAmbiencePitch;
            _secondAmbienceSource.loop = nextAmbience.Sound.IsLooped;
            _secondAmbienceSource.Play();

            _blendingAmbienceSequence = DOTween.Sequence();
            _blendingAmbienceSequence.Append(_firstAmbienceSource.DOFade(0f, nextAmbience.BlendTIme));

            if (nextAmbience.DoOverlap)
            {
                _blendingAmbienceSequence.Join(_secondAmbienceSource.DOFade(nextAmbience.Sound.Volume, nextAmbience.BlendTIme));
            }
            else
            {
                _blendingAmbienceSequence.Append(_secondAmbienceSource.DOFade(nextAmbience.Sound.Volume, nextAmbience.BlendTIme));
            }

            _blendingAmbienceSequence.OnComplete(SwitchAmbienceSources);
        }

        private void SwitchAmbienceSources()
        {
            _ambienceQueue.Dequeue();
            (_firstAmbienceSource, _secondAmbienceSource) = (_secondAmbienceSource, _firstAmbienceSource);
            _secondAmbienceSource.Stop();
            PlayNextAmbienceList();
        }

        private void PlayEffectSound(IReadOnlyList<AudioSource> sources, Sound sound, Vector3 position = default, float distance = 0f)
        {
            if (!_areEffectsSoundsEnabled || sound == null) return;
            for (var i = 0; i < sources.Count; i++)
            {
                if (!sources[i].isPlaying)
                {
                    SetUpSourceSettings(sources[i], sound, position, distance);
                    sources[i].Play();
                    
                    return;
                }

                Debug.LogWarning("don't have enough sources");
            }
        }

        private void SetUpSourceSettings(AudioSource audioSource, Sound sound, Vector3 position = default, float distance = 0f)
        {
            audioSource.transform.position = position;
            audioSource.outputAudioMixerGroup = sound.MixerGroup;
            audioSource.clip = sound.SoundClip;
            audioSource.volume = sound.DoRandomVolume ? Random.Range(sound.VolumeRange.x, sound.VolumeRange.y) : sound.Volume;
            audioSource.pitch = sound.DoRandomPitch ? Random.Range(sound.PitchRange.x, sound.PitchRange.y) : sound.Pitch;
            audioSource.priority = sound.DoRandomPriority ? Random.Range(sound.PriorityRange.x, sound.PriorityRange.y) : sound.Priority;
            audioSource.loop = sound.IsLooped;
            audioSource.maxDistance = distance;
            audioSource.dopplerLevel = sound.DopplerLevel;
        }
        
    }
}