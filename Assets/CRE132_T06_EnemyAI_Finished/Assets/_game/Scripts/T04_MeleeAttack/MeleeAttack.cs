// Press Attack to swing a sword the way the character faces. The swing's animation clips call HitboxOn and
// HitboxOff with Animation Events, so the hit happens on the frames where the sword is out.
// Put it on the object with the Animator (the events call methods on that object).
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private float cooldown = 0.5f;

    // How long the character stands still for a swing (the length of the attack clips).
    [SerializeField] private float attackTime = 0.35f;

    // The sword's hit area: a child object with a trigger collider and AttackHitbox, switched off between swings.
    [SerializeField] private GameObject hitbox;
    [SerializeField] private float hitboxDistance = 0.8f;
    // Where swings start from, measured from this object: low on the body, because in a top-down game
    // characters' colliders are at their feet.
    [SerializeField] private Vector2 swingOrigin = new Vector2(0f, -0.8f);

    private Animator animator;
    private DirectionalAnimator directionalAnimator;
    private TopDownMovement movement;
    private float nextAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        directionalAnimator = GetComponent<DirectionalAnimator>();
        movement = GetComponentInParent<TopDownMovement>();
        hitbox.SetActive(false);
    }

    private void OnEnable()
    {
        attackAction.action.Enable();
    }

    private void OnDisable()
    {
        attackAction.action.Disable();
    }

    private void Update()
    {
        if (attackAction.action.WasPressedThisFrame() && Time.time >= nextAttackTime)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        // The cooldown stops button-mashing from swinging faster than the animation.
        nextAttackTime = Time.time + cooldown;
        movement.enabled = false;
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackTime);

        movement.enabled = true;
    }

    // Called by an Animation Event on the swing frame.
    public void HitboxOn()
    {
        Vector2 offset = swingOrigin + directionalAnimator.LastDirection * hitboxDistance;
        hitbox.transform.localPosition = offset;
        hitbox.SetActive(true);
    }

    // Called by an Animation Event when the swing is over.
    public void HitboxOff()
    {
        hitbox.SetActive(false);
    }
}
