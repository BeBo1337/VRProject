using System.Collections.Generic;
using System.Linq;
using DataTypes;
using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambianceMusicAudioSource; 
        [SerializeField] private AudioSource _ambianceSFXAudioSource; 
        [SerializeField] private List<SoundDefinition> _soundDefinitions;
        
        private Dictionary<SoundType, AudioClip> _soundDefinitionsLookup; 

        public static AudioManager Instance { get; private set; } // Singleton instance
        
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                // Create a dictionary for fast lookup of sound clips by type
                _soundDefinitionsLookup =
                    _soundDefinitions.ToDictionary(key => key.SoundType, value => value.AudioClip);
            }
            else
            {
                Destroy(gameObject); // Destroy duplicate AudioManager instances
            }
        }

        // Play a sound effect using a specified AudioSource and SoundType
        public void PlaySoundEffect(AudioSource audioSource, SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                audioSource.PlayOneShot(audioClip); // Play the sound effect once
            }
        }

        // Get the AudioClip associated with a SoundType
        private AudioClip GetSound(SoundType soundType)
        {
            _soundDefinitionsLookup.TryGetValue(soundType, out var audioClip);
            return audioClip;
        }

        // Play a sound effect using the ambianceSFXAudioSource
        public void PlaySoundEffect(SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                _ambianceSFXAudioSource.PlayOneShot(audioClip); // Play the sound effect once
            }
        }
        
        // Play music using the ambianceMusicAudioSource
        public void PlayMusic(SoundType soundType)
        {
            var audioClip = GetSound(soundType);
            if (audioClip != null)
            {
                _ambianceMusicAudioSource.PlayOneShot(audioClip); // Play the music once
            }
        }

        // Stop the music being played on the ambianceMusicAudioSource
        public void StopMusic()
        {
            _ambianceMusicAudioSource.Stop();
        }
    }
}
