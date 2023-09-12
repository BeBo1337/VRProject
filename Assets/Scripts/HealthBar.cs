using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform  healthBarFill;
    [SerializeField] private float maxWidth = 300f; // Adjust the maximum health as needed

    public void UpdateHealth(float currentHealth)
    {
        // Ensure that the new health value stays within the valid range (0 - maxHealth)
        float clampedHealth = Mathf.Clamp(currentHealth, 0f, maxWidth);

        // Calculate the health percentage and update the fill width
        float healthPercentage = clampedHealth / maxWidth;
        float newWidth = maxWidth * healthPercentage;

        // Update the sizeDelta.x of the healthBarFill RectTransform
        Vector2 newSizeDelta = healthBarFill.sizeDelta;
        newSizeDelta.x = newWidth * 3;
        healthBarFill.sizeDelta = newSizeDelta;
    }
}
