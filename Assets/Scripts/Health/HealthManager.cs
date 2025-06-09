// HealthManager.cs
using UnityEngine;

public class HealthManager
{
    public int maxHealth { get; private set; }
    public int currentHealth { get; private set; }

    public delegate void OnDeath();
    public event OnDeath onDeath;

    public HealthManager(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            onDeath?.Invoke();
        }
    }

    /*
    public void SetMaxHealth(int newMax, bool healToFull = false)
    {
        maxHealth = newMax;

        if (healToFull)
            currentHealth = maxHealth;
        else if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
    */

}
