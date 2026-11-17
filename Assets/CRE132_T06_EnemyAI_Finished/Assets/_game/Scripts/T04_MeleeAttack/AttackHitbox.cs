// The sword's hit area. While it's switched on, anything with Health that it touches takes damage,
// but only once per swing, even if it stays in the area for several frames.
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private List<Health> alreadyHit = new List<Health>();

    // Switched on at the start of each swing: forget who was hit last time.
    private void OnEnable()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // InParent finds the Health on the object or above it (a character's Health is on its root).
        Health health = other.GetComponentInParent<Health>();
        if (health == null || alreadyHit.Contains(health))
        {
            return;
        }

        // Don't hit ourselves.
        if (health.transform == transform.root)
        {
            return;
        }

        alreadyHit.Add(health);
        health.TakeDamage(damage);
    }
}
