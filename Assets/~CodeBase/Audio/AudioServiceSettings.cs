using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CodeBase.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioServiceSettings))]
    public sealed class AudioServiceSettings : ScriptableObject
    {
        [SerializeField] private AudioMixer _mainMixer;
        
        [ListDrawerSettings(ListElementLabelName = "SoundName")]
        [SerializeField] private Sound[] _musicSounds;
        
        [ListDrawerSettings(ListElementLabelName = "SoundName")]
        [SerializeField] private Sound[] _ambientSounds;
        
        [ListDrawerSettings(ListElementLabelName = "SoundName")]
        [SerializeField] private Sound[] _effectSounds;


        public IEnumerable<Sound> MusicSounds => _musicSounds;
        public IEnumerable<Sound> AmbienceSounds => _ambientSounds;
        public IEnumerable<Sound> EffectsSounds => _effectSounds;
        
        public AudioMixer MainMixer => _mainMixer;


        public Sound GetMusicSoundByName(string soundName)
        {
            return MusicSounds.FirstOrDefault(x => x.SoundName == soundName);
        }

        public Sound GetAmbienceSoundByName(string soundName)
        {
            return AmbienceSounds.FirstOrDefault(x => x.SoundName == soundName);
        }

        public Sound GetEffectSoundByName(string soundName)
        {
            return EffectsSounds.FirstOrDefault(x => x.SoundName == soundName);
        }


#if UNITY_EDITOR
        public static IEnumerable<string> GetAllAudioNames()
        {
            var instance = AssetDatabase.LoadAssetAtPath<AudioServiceSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t: {typeof(AudioServiceSettings).FullName}").First())); 
            return instance._musicSounds.Concat(instance._ambientSounds).Concat(instance._effectSounds).Select(s => s.SoundName);
        }
#endif
    }
}