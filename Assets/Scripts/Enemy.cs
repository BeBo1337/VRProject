using DataTypes;
using Managers;
using UnityEngine;
using XRCardboard;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Animator _animator; 
    [SerializeField] private AudioSource _audioSource;

    private int _health;
    private bool _isMoving;
    public bool isDead;

    private void Update()
    {
        MoveTowardsPlayer();
    }
    
    private void MoveTowardsPlayer()
    {
        if (_isMoving)
        {
            var target = XRCardboardController.Instance.CameraTransform;
            var targetPosition = target.position;
            targetPosition.y = 0;

            // Calculate the new position as before
            var newPosition = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
            _rotatingTransform.position = newPosition;

            // Make the object look at the target position
            _rotatingTransform.LookAt(targetPosition);
        }
    }

    public void UpdateMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }
    
    public void Initialize(Vector3 spawnPointPosition, int startingHealth)
    {
        _rotatingTransform.position = spawnPointPosition;
        _health = startingHealth;
    }
    
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
                _health -= 1;
                break;
        }
        var shouldDestory = _health <= 0;
        return shouldDestory;
    }

    public void Die()
    {
        // AudioManager.Instance.PlaySoundEffect(SoundType.EnemyExplosion);
        // TODO: effect , sound , animation 
        
        isDead = true;
        _isMoving = false;
        _animator.Play("ZombieDie");
    }
    
    public void OnZombieDieAnimationComplete()
    {
        EnemySpawner.BaseInstance.Release(this);
        isDead = false;
    }

    public void Hit()
    {
        _isMoving = false;
        _animator.Play("ZombieHit");
    }
}