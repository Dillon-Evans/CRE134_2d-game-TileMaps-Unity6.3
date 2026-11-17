// Turns the sprite redder as health drops, so damage shows without any UI (S03 adds a health bar).
using UnityEngine;

public class HealthTint : MonoBehaviour
{
    [SerializeField] private Color hurtColour = new Color(1f, 0.25f, 0.25f);

    private Health health;
    private SpriteRenderer spriteRenderer;
    private Color healthyColour;

    private void Awake()
    {
        health = GetComponent<Health>();
        // InChildren also finds a sprite on a child object (the top-down characters from T03 keep it there).
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        healthyColour = spriteRenderer.color;
    }

    private void Update()
    {
        // 1 at full health, 0 at none. The cast to float stops whole-number division (2 / 3 would be 0).
        float healthFraction = (float)health.CurrentHealth / health.MaxHealth;
        spriteRenderer.color = Color.Lerp(hurtColour, healthyColour, healthFraction);
    }
}
