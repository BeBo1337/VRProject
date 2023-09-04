using DataTypes;
using Managers;
using UnityEngine;
using XRCardboard;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private AudioSource _audioSource;

    private int _health;
    private bool _isMoving;

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
    
    public bool ReduceHealth()
    {
        _health--;
        var shouldDestory = _health <= 0;
        return shouldDestory;
    }

    public void Die()
    {
        // AudioManager.Instance.PlaySoundEffect(SoundType.EnemyExplosion);
        // TODO: effect , sound , animation 
    }
}