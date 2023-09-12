using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public HealthBar healthBar;

    private void Awake()
    {
        // Initialize the health bar
        currentHealth = 100;
        healthBar.UpdateHealth(currentHealth);
    }
    
    public void TakeDamage()
    {
        // Subtract 25 hp on hit from current health
        currentHealth -= 25f;

        // Update the health bar with the new health value
        healthBar.UpdateHealth(currentHealth);
    }
}