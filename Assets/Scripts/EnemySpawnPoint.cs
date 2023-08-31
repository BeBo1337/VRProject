using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
}