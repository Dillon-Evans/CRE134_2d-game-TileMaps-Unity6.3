// Sends the direction of movement to the Animator's blend trees (DirectionX, DirectionY) and the speed (Speed).
// It remembers the last direction, so the character keeps facing that way when it stops.
// Put it on the object with the Animator; it finds the Rigidbody2D on itself or a parent.
using UnityEngine;

public class DirectionalAnimator : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;

    // Facing down to start with: (0, -1).
    private Vector2 lastDirection = Vector2.down;

    // Other scripts (the T04 attack) use this to know which way the character faces.
    public Vector2 LastDirection
    {
        get { return lastDirection; }
    }

    private void Awake()
    {
        body = GetComponentInParent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 velocity = body.linearVelocity;
        float speed = velocity.magnitude;

        if (speed > 0.1f)
        {
            // Pixel art can't blend between two directions, so pick the main one:
            // left/right if moving more sideways, up/down if moving more vertically.
            if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
            {
                lastDirection = new Vector2(Mathf.Sign(velocity.x), 0f);
            }
            else
            {
                lastDirection = new Vector2(0f, Mathf.Sign(velocity.y));
            }
        }

        animator.SetFloat("Speed", speed);
        animator.SetFloat("DirectionX", lastDirection.x);
        animator.SetFloat("DirectionY", lastDirection.y);
    }
}
