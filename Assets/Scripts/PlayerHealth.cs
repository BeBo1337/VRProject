using Managers;
using UnityEngine;

// Health Logic
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float _playerHealth;
    [SerializeField] private float _damageHit;
    public HealthBar healthBar;

    private void Awake()
    {// Initialize the health bar
        healthBar.UpdateHealth(_playerHealth);
    }
    
    public void TakeDamage()
    {
        // Subtract 25 hp on hit from current health
        _playerHealth -= _damageHit;
        
        // Update the health bar with the new health value
        healthBar.UpdateHealth(_playerHealth);
        
        if(_playerHealth <= 0)
            GameManager.Instance.SetGameOver();

    }
}