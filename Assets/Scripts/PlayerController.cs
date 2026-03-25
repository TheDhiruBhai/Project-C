using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public PlayerControls Controls { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float lookSpeed = 15f;

    [Header("Components")]
    public Camera playerCamera; // drag your camera here in inspector

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintHeld;

    private void Awake()
    {
        // Only setup controls if this is OUR player
        if (!photonView.IsMine) return;

        Controls = new PlayerControls();

        string rebinds = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(rebinds))
            Controls.LoadBindingOverridesFromJson(rebinds);

        Controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        Controls.Player.Move.canceled += _ => moveInput = Vector2.zero;
        Controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        Controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            // This is OUR player — keep camera on
            if (playerCamera != null)
            {
                playerCamera.enabled = true;
                playerCamera.tag = "MainCamera";
            }

            Debug.Log("Local player: " + PhotonNetwork.NickName);
        }
        else
        {
            // This is SOMEONE ELSE — disable camera
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);

            Debug.Log("Remote player: " + photonView.Owner.NickName);
        }
    }

    private void OnEnable()
    {
        if (photonView.IsMine && Controls != null)
            Controls.Enable();
    }

    private void OnDisable()
    {
        if (photonView.IsMine && Controls != null)
            Controls.Disable();
    }

    private void Update()
    {
        // Only move if this is OUR player
        if (!photonView.IsMine) return;

        Vector2 m = moveInput;
        if (m.sqrMagnitude > 1f) m = m.normalized;

        float currentSpeed = moveInput.y > 0 && sprintHeld ? sprintSpeed : moveSpeed;

        Vector3 direction = transform.forward * m.y + transform.right * m.x;
        transform.position += direction * currentSpeed * Time.deltaTime;

        float yaw = lookInput.x * lookSpeed * Time.deltaTime;
        transform.Rotate(0f, yaw, 0f);
    }
}