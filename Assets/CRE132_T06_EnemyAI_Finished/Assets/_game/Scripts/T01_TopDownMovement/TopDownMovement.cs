// Moves a top-down character in eight directions with the Input System.
// The Rigidbody2D has Gravity Scale 0, so nothing pulls it down the screen.
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    // Keeps diagonal movement the same speed as straight movement. The Input System's WASD composite
    // already does this (its Mode is Digital Normalized); to see the problem, set that Mode to Digital
    // in the Input Actions asset and untick this.
    [SerializeField] private bool normaliseDiagonals = true;

    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D body;
    private Vector2 moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        // Stop when switched off (for example while attacking), or the body would keep sliding.
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();

        // Up + Right on a keyboard gives (1, 1), which is 1.41 long, so diagonals would be faster.
        // Normalising makes it 1 long. Only longer inputs are shortened, so a gamepad stick pushed
        // part of the way still moves slowly.
        if (normaliseDiagonals && moveInput.magnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity = moveInput * moveSpeed;
    }
}
