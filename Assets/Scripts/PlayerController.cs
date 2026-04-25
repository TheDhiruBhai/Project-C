using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Photon.Pun;
using Game.Player;
using System;

public class PlayerController : MonoBehaviourPun
{
    public PlayerControls Controls { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float lookSpeed = 15f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxLookAngle = 90f;

    private float verticalVelocity;
    private float currentPitch = 0f;
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintHeld;
    private bool inputReady = false;
    private PlayerSpeedModifier speedModifier;

    public static event Action GrimoireControl;

    public static event Action GrimoireControl;

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
        if (!photonView.IsMine)
        {
            this.enabled = false;
            return;
        }

        Controls = new PlayerControls();
        controller = GetComponent<CharacterController>();
        speedModifier = GetComponentInChildren<PlayerSpeedModifier>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        string rebinds = PlayerPrefs.GetString("rebinds", "");
            if (!string.IsNullOrEmpty(rebinds))
            Controls.LoadBindingOverridesFromJson(rebinds);

        Controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        Controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        Controls.Player.Look.performed += ctx =>
        {
            if (!inputReady) return;           // ← ignore until ready
            lookInput = ctx.ReadValue<Vector2>();
        };

        Controls.Player.Look.canceled += _ => lookInput = Vector2.zero;

        Controls.Player.Jump.performed += _ => TryJump();

        Cursor.visible = false;
    }

    private IEnumerator Start()
    {
        yield return null;          // skip one frame so the initial mouse event fires and is swallowed
        lookInput = Vector2.zero;   // hard reset just in case
        inputReady = true;
    }

    private void OnEnable() => Controls.Enable();
    private void OnDisable() => Controls.Disable();

    private void Update()
    {
        Vector2 m = moveInput;
        if (m.sqrMagnitude > 1f) m = m.normalized;

        float currentSpeed = moveInput.y > 0 && sprintHeld ? sprintSpeed : moveSpeed;
        if (speedModifier != null)
            currentSpeed *= speedModifier.CurrentMultiplier;

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

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float yaw = lookInput.x * lookSpeed * Time.deltaTime;
            transform.Rotate(0f, yaw, 0f);

            float pitch = lookInput.y * lookSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch - pitch, -maxLookAngle, maxLookAngle);
            mainCamera.transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Activate pause menu here
            GrimoireControl?.Invoke();
            bool isLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isLocked;
            //GrimoireControl?.Invoke();
        }
    
     }
}