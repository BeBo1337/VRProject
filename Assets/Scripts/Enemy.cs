using DataTypes;
using Managers;
using UnityEngine;
using UnityEngine.UI;

// Enemy Logic
public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _zombieTransform;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Animator _animator; 
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Slider _hpslider;
    public AudioSource AudioSource => _audioSource;
    
    private PlayerHealth _playerHealth;
    private float _health;
    private float _maxhealth;
    private bool _isMoving;
    public bool isDead;

    private const string ZombieHitAnimationString = "ZombieHit"; 
    private const string ZombieDieAnimationString = "ZombieDie"; 
    private const string MoveSpeedString = "moveSpeed"; 
    private const string PlayerTagString = "Player"; 
    private const string AttackString = "attack"; 

    private void Awake()
    {
        if (_playerHealth == null)
            _playerHealth = FindObjectOfType<PlayerHealth>();
        
        _audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource reference
    }

    private void Update()
    {   
        // Function to make the enemy move towards the player
        MoveTowardsPlayer(); 
    }

    private void MoveTowardsPlayer()
    {
        if (_isMoving)
        {
            var target = Camera.main.transform; 
            var targetPosition = target.position;

            // Adjust the target position based on the game level
            if (GameManager._level == 1)
                targetPosition.y = 0;
            else 
                targetPosition.y = 4.1f;

            // Calculate the new position for the enemy
            var newPosition = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
            _zombieTransform.position = newPosition;
            _zombieTransform.LookAt(targetPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the enemy has collided with the player
        if (other.gameObject.tag.Equals(PlayerTagString))
        {
            _isMoving = false; // Stop moving
            _animator.SetBool(AttackString, true); // Trigger attack animation
        }
    }

    // Function to initialize the enemy
    public void Initialize(Vector3 spawnPointPosition, int startingHealth, float speed)
    {
        _zombieTransform.LookAt(Camera.main.transform); // Make the enemy face the camera
        _zombieTransform.position = spawnPointPosition; // Set the enemy's initial position
        _health = _maxhealth = _hpslider.value = _hpslider.maxValue = startingHealth;
        
        _movementSpeed = speed; 
        _animator.SetFloat(MoveSpeedString, speed); 
        _animator.SetBool(AttackString, false); // Initialize attack animation as false (not attacking)
    }

    // Function to reduce the enemy's health based on the body part hit
    public bool ReduceHealth(HitType bodyPartHit)
    {
        _hpslider.gameObject.SetActive(true); // Show hp bar after being hit at least once
        switch (bodyPartHit)
        {
            case HitType.HeadShot:
                _health -= 5;
                break;
            case HitType.BodyShot:
                _health -= 3;
                break;
            case HitType.LimbShot:
                _health -= 1;
                break;
        }

        var shouldDestroy = _health <= 0;
        if (shouldDestroy)
            _hpslider.value = 0;
        else
            _hpslider.value = _health;
        
        return shouldDestroy;
    }

    // Function to handle the enemy's death
    public void Die()
    {
        if (!GameManager._gameOver)
            AudioManager.Instance.PlaySoundEffect(_audioSource, SoundType.EnemyDying);

        isDead = true; 
        _isMoving = false; 
        _animator.Play(ZombieDieAnimationString); // Play the death animation
    }

    // Function to handle the enemy being hit
    public void Hit()
    {
        AudioManager.Instance.PlaySoundEffect(_audioSource, SoundType.EnemyHit); // Play hit sound effect.
        _isMoving = false;
        _animator.Play(ZombieHitAnimationString); // Play the hit animation
    }

    // Function called when the zombie die animation is complete
    public void OnZombieDieAnimationComplete()
    {
        _hpslider.gameObject.SetActive(false); // Reset hp bar to false
        isDead = false; // Reset the dead flag
        EnemySpawner.BaseInstance.Release(this); // Release the enemy
    }

    // Function called when the zombie hit animation is complete
    public void OnZombieHitAnimationComplete()
    {
        _isMoving = true; 
    }

    // Function called when the zombie attack animation is complete
    public void OnZombieAttackAnimationComplete()
    {
        _playerHealth.TakeDamage();
    }
}