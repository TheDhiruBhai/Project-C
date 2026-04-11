// ============================================================
//  HoloCardController.cs
//  Attach to the card GameObject alongside a MeshRenderer using
//  the PokemonHoloCard shader.
//
//  Features:
//   - Drives view-angle shader parameters from the camera's
//     real position each frame (makes the holo react to camera movement)
//   - Optional card tilt animation (e.g. for menu screens)
//   - Inspector-friendly live preview
// ============================================================

using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HoloCardController : MonoBehaviour
{
    // ── Holo feel ──────────────────────────────────────────────
    [Header("Holo Intensity")]
    [Range(0f, 3f)]  public float holoStrength   = 1.2f;
    [Range(0f, 1f)]  public float holoSaturation = 0.85f;
    [Range(0f, 2f)]  public float holoBrightness = 1.0f;
    [Range(0f, 2f)]  public float animSpeed      = 0.15f;

    [Header("Sparkle")]
    [Range(0f, 5f)]  public float sparkleStrength = 2.5f;
    [Range(0f, 1f)]  public float sparkleCutoff   = 0.75f;

    [Header("Fresnel")]
    [Range(0.5f, 8f)] public float fresnelPower    = 3.0f;
    [Range(0f, 3f)]   public float fresnelStrength = 1.0f;

    // ── Card tilt animation ────────────────────────────────────
    [Header("Tilt Animation (optional)")]
    public bool  animateTilt     = false;
    [Range(0f, 45f)] public float tiltAmplitude = 12f;   // degrees
    [Range(0f, 2f)]  public float tiltSpeed     = 0.8f;

    // ── Mouse-follow (for UI/showcase mode) ───────────────────
    [Header("Mouse Tilt (showcase mode)")]
    public bool  mouseFollowTilt = false;
    [Range(0f, 30f)] public float mouseTiltStrength = 15f;
    public float mouseSmoothTime = 0.12f;

    // ── Cached refs ────────────────────────────────────────────
    private Renderer   _renderer;
    private MaterialPropertyBlock _mpb;
    private Camera     _cam;

    private Vector2 _mouseTiltTarget;
    private Vector2 _mouseTiltCurrent;
    private Vector2 _mouseTiltVelocity;

    // Shader property IDs (cached for performance)
    private static readonly int IDHoloStrength    = Shader.PropertyToID("_HoloStrength");
    private static readonly int IDHoloSaturation  = Shader.PropertyToID("_HoloSaturation");
    private static readonly int IDHoloBrightness  = Shader.PropertyToID("_HoloBrightness");
    private static readonly int IDAnimSpeed       = Shader.PropertyToID("_AnimSpeed");
    private static readonly int IDSparkleStrength = Shader.PropertyToID("_SparkleStrength");
    private static readonly int IDSparkleCutoff   = Shader.PropertyToID("_SparkleCutoff");
    private static readonly int IDFresnelPower    = Shader.PropertyToID("_FresnelPower");
    private static readonly int IDFresnelStrength = Shader.PropertyToID("_FresnelStrength");

    // ── Lifecycle ──────────────────────────────────────────────
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb      = new MaterialPropertyBlock();
        _cam      = Camera.main;
    }

    void Update()
    {
        // Push parameters to shader without creating material instances
        _renderer.GetPropertyBlock(_mpb);

        _mpb.SetFloat(IDHoloStrength,    holoStrength);
        _mpb.SetFloat(IDHoloSaturation,  holoSaturation);
        _mpb.SetFloat(IDHoloBrightness,  holoBrightness);
        _mpb.SetFloat(IDAnimSpeed,       animSpeed);
        _mpb.SetFloat(IDSparkleStrength, sparkleStrength);
        _mpb.SetFloat(IDSparkleCutoff,   sparkleCutoff);
        _mpb.SetFloat(IDFresnelPower,    fresnelPower);
        _mpb.SetFloat(IDFresnelStrength, fresnelStrength);

        _renderer.SetPropertyBlock(_mpb);

        // ── Tilt animation ──
        if (animateTilt && !mouseFollowTilt)
        {
            float t       = Time.time * tiltSpeed;
            float pitchDeg = Mathf.Sin(t)           * tiltAmplitude;
            float yawDeg   = Mathf.Sin(t * 0.7f + 1f) * tiltAmplitude * 0.6f;
            transform.localRotation = Quaternion.Euler(pitchDeg, yawDeg, 0f);
        }

        // ── Mouse follow tilt (for showcase/UI) ──
        if (mouseFollowTilt && _cam != null)
        {
            // Convert mouse position to normalised viewport coords centred at 0,0
            Vector3 mouseVP = _cam.ScreenToViewportPoint(Input.mousePosition);
            _mouseTiltTarget = new Vector2(
                (mouseVP.x - 0.5f) * 2f,
                (mouseVP.y - 0.5f) * 2f
            );

            _mouseTiltCurrent = Vector2.SmoothDamp(
                _mouseTiltCurrent,
                _mouseTiltTarget,
                ref _mouseTiltVelocity,
                mouseSmoothTime
            );

            float pitchDeg = -_mouseTiltCurrent.y * mouseTiltStrength;
            float yawDeg   =  _mouseTiltCurrent.x * mouseTiltStrength;
            transform.localRotation = Quaternion.Euler(pitchDeg, yawDeg, 0f);
        }
    }

    // Editor helper: reset tilt when both options are off
    void OnValidate()
    {
        if (!animateTilt && !mouseFollowTilt && Application.isEditor)
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}
