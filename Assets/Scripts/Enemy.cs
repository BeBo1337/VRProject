using DataTypes;
using Managers;
using UnityEngine;
using XRCardboard;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private AudioSource _audioSource;

    private int _health;
    private bool _isMoving;

    private void Update()
    {
        RotateAroundSelf();
        MoveTowardsPlayer();
    }

    private void RotateAroundSelf()
    {
        var rotationDelta = _rotationSpeed * Time.deltaTime;
        _rotatingTransform.Rotate(Vector3.up, rotationDelta);
    }
    
    private void MoveTowardsPlayer()
    {
        if (_isMoving)
        {
            var target = XRCardboardController.Instance.CameraTransform;
            var newPosition = Vector3.MoveTowards(transform.position, target.position, 
                _movementSpeed * Time.deltaTime);
            _rotatingTransform.position = newPosition;   
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
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        var material = GameManager.Instance.GetEnemyMaterial(_health);
        if (material != null)
        {
            _renderer.material = material;   
        }
    }

    public bool ReduceHealth()
    {
        _health--;
        UpdateMaterial();
        var shouldDestory = _health <= 0;
        return shouldDestory;
    }

    public void Explode()
    {
        AudioManager.Instance.PlaySoundEffect(SoundType.EnemyExplosion);
        // TODO: effect
    }
}