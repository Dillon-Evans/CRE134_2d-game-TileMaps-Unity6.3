// Health that anything can have: the player, enemies, breakable crates.
// Other scripts call TakeDamage and Heal; this script decides what happens at zero.
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    // Untick for a player who respawns (P10): the object stays, and something else handles death.
    [SerializeField] private bool destroyOnDeath = true;

    // UnityEvents: lists of things to do, filled in the Inspector (play a sound, show a panel...).
    [SerializeField] private UnityEvent onDamaged = new UnityEvent();
    [SerializeField] private UnityEvent onHealed = new UnityEvent();
    [SerializeField] private UnityEvent onDied = new UnityEvent();

    // A C# event: other scripts subscribe to it in code (health.HealthChanged += MyMethod).
    // The two numbers sent are the current and maximum health.
    public event System.Action<int, int> HealthChanged;

    private int currentHealth;

    // Read-only from outside: other scripts can look at health but must use TakeDamage or Heal to change it.
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        // Already dead: ignore any more hits that arrive in the same frame.
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth = currentHealth - amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        Debug.Log(gameObject.name + " took " + amount + " damage, health " + currentHealth + "/" + maxHealth);
        onDamaged.Invoke();
        SendHealthChanged();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = currentHealth + amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log(gameObject.name + " healed " + amount + ", health " + currentHealth + "/" + maxHealth);
        onHealed.Invoke();
        SendHealthChanged();
    }

    // Sets health directly, for loading a saved game. Kept between 1 and the maximum.
    public void SetHealth(int amount)
    {
        currentHealth = Mathf.Clamp(amount, 1, maxHealth);
        SendHealthChanged();
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died");
        onDied.Invoke();
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }

    // Back to full health, for respawning.
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        SendHealthChanged();
    }

    private void SendHealthChanged()
    {
        // Nobody may be listening: an event with no subscribers is null.
        if (HealthChanged != null)
        {
            HealthChanged(currentHealth, maxHealth);
        }
    }
}
