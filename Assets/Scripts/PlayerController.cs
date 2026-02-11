using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerControls Controls { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float lookSpeed = 15f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    private float verticalVelocity;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintHeld;

    private bool IsGrounded()
    {
        return controller.isGrounded;
    }

    private void TryJump()
    {
        if (!IsGrounded()) return;

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Awake()
    {
        Controls = new PlayerControls();
        controller = GetComponent<CharacterController>();

        string rebinds = PlayerPrefs.GetString("rebinds", "");
            if (!string.IsNullOrEmpty(rebinds))
            Controls.LoadBindingOverridesFromJson(rebinds);

        Controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        Controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        Controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        Controls.Player.Look.canceled += _ => lookInput = Vector2.zero;

        Controls.Player.Jump.performed += _ => TryJump();
    }

    private void OnEnable() => Controls.Enable();
    private void OnDisable() => Controls.Disable();

    private void Update()
    {
        Vector2 m = moveInput;
        if (m.sqrMagnitude > 1f) m = m.normalized;

        float currentSpeed = moveInput.y > 0 && sprintHeld ? sprintSpeed : moveSpeed;

        Vector3 horizontal =
        transform.forward * m.y +
        transform.right * m.x;

        if (IsGrounded() && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity =
            horizontal * currentSpeed +
            Vector3.up * verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        float yaw = lookInput.x * lookSpeed * Time.deltaTime;
        transform.Rotate(0f, yaw, 0f);
    }
}
