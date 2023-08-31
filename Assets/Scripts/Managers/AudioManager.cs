using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambianceMusicAudioSource;
        [SerializeField] private AudioSource _ambianceSFXAudioSource;
        [SerializeField] private List<SoundDefinition> _soundDefinitions;
        
        private Dictionary<SoundType, AudioClip> _soundDefinitionsLookup;

        public static AudioManager Instance { get; private set; }
        
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _soundDefinitionsLookup =
                    _soundDefinitions.ToDictionary(key => key.SoundType, value => value.AudioClip);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySoundEffect(AudioSource audioSource, SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        private AudioClip GetSound(SoundType soundType)
        {
            _soundDefinitionsLookup.TryGetValue(soundType, out var audioClip);
            return audioClip;

        }

        public void PlaySoundEffect(SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                _ambianceSFXAudioSource.PlayOneShot(audioClip);
            }
        }
        
        public void PlayMusic(SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                _ambianceMusicAudioSource.PlayOneShot(audioClip);
            }
        }

        public void StopMusic()
        {
            _ambianceMusicAudioSource.Stop();
        }
    }
}