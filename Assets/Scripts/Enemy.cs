using DataTypes;
using Managers;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _zombieTransform;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Animator _animator; 
    [SerializeField] private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;
    
    private PlayerHealth _playerHealth;
    private int _health;
    private bool _isMoving;
    public bool isDead;

    private const string ZombieHitAnimationString = "ZombieHit"; 
    private const string ZombieDieAnimationStrin = "ZombieDie"; 
    private const string MoveSpeedString = "moveSpeed"; 
    private const string PlayerTagString = "Player"; 
    private const string AttackString = "attack"; 

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>(); // Initialize the AudioSource reference
    }

    private void Update()
    {// Function to make the enemy move towards the player
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
            else if (GameManager._level == 2)
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
        if (_playerHealth == null)
            _playerHealth = FindObjectOfType<PlayerHealth>();

        _zombieTransform.LookAt(Camera.main.transform); // Make the enemy face the camera
        _zombieTransform.position = spawnPointPosition; // Set the enemy's initial position
        _health = startingHealth; 
        _movementSpeed = speed; 
        _animator.SetFloat(MoveSpeedString, speed); 
        _animator.SetBool(AttackString, false); // Initialize attack animation as false (not attacking)
    }

    // Function to reduce the enemy's health based on the body part hit
    public bool ReduceHealth(HitType bodyPartHit)
    {
        switch (bodyPartHit)
        {
            case HitType.HeadShot:
                _health -= 5;
                break;
            case HitType.BodyShot:
                _health -= 3;
                break;
            case HitType.LimbShot:
                _health -= 2;
                break;
        }

        var shouldDestroy = _health <= 0;
        return shouldDestroy;
    }

    // Function to handle the enemy's death
    public void Die()
    {
        if (!GameManager._gameOver)
            AudioManager.Instance.PlaySoundEffect(_audioSource, SoundType.EnemyDying);

        isDead = true; 
        _isMoving = false; 
        _animator.Play(ZombieDieAnimationStrin); // Play the death animation
    }

    // Function to handle the enemy being hit
    public void Hit()
    {
        _isMoving = false;
        _animator.Play(ZombieHitAnimationString); // Play the hit animation
    }

    // Function called when the zombie die animation is complete
    public void OnZombieDieAnimationComplete()
    {
        EnemySpawner.BaseInstance.Release(this); // Release the enemy
        isDead = false; // Reset the dead flag
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