using System;
using UnityEngine;

public class Health : MonoBehaviour 
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] FloatEventChannel playerHealthChannel;

    int currentHealth;

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PublishHealthPercentage();
    }

    private void PublishHealthPercentage()
    {
        if(playerHealthChannel != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            playerHealthChannel.Invoke(healthPercentage);
        }
    }
}
