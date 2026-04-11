// ============================================================
//  PokemonHoloCard.shader — URP Forward Lit
//  Holographic foil shader mimicking Pokemon gold card effect.
//
//  Effects:
//   1. Iridescent rainbow  — view-angle & UV driven hue shift
//   2. Diagonal stripe pattern — holographic diffraction lines
//   3. Sparkle / glitter     — noise-based point highlights
//   4. Fresnel rim           — edge rainbow
//   5. Holo mask             — white = full holo, black = base card only
//
//  Setup:
//   - Assign to a Quad or Plane mesh sized to card aspect (2.5 x 3.5)
//   - _MainTex  : your card art texture
//   - _HoloMask : grayscale mask (white = holographic area)
//   - _NormalMap: (optional) normal map for surface micro-detail
//   - _SparkleNoise : tileable grayscale noise (e.g. blue noise or RGB noise)
// ============================================================

Shader "Custom/PokemonHoloCard"
{
    Properties
    {
        [Header(Card Textures)]
        _MainTex        ("Card Texture",      2D)    = "white" {}
        _HoloMask       ("Holo Mask (R)",     2D)    = "white" {}
        _NormalMap      ("Normal Map",        2D)    = "bump"  {}
        _SparkleNoise   ("Sparkle Noise",     2D)    = "white" {}

        [Header(Iridescent Rainbow)]
        _HoloStrength   ("Holo Strength",         Range(0, 3))  = 1.2
        _RainbowTiling  ("Rainbow Tiling",         Range(1, 30)) = 10.0
        _ViewShift      ("View Angle Influence",   Range(0, 2))  = 0.8
        _AnimSpeed      ("Animation Speed",        Range(0, 2))  = 0.15

        [Header(Stripe Pattern)]
        _StripeCount    ("Stripe Count",           Range(2, 40)) = 18.0
        _StripeAngle    ("Stripe Angle (deg)",     Range(0, 90)) = 30.0
        _StripeSharpness("Stripe Sharpness",       Range(1, 8))  = 3.0

        [Header(Sparkle Glitter)]
        _SparkleStrength("Sparkle Strength",       Range(0, 5))  = 2.5
        _SparkleScale   ("Sparkle Scale",          Range(5, 80)) = 30.0
        _SparkleCutoff  ("Sparkle Cutoff",         Range(0, 1))  = 0.75
        _SparkleSharp   ("Sparkle Sharpness",      Range(1, 20)) = 10.0

        [Header(Fresnel Rim)]
        _FresnelPower   ("Fresnel Power",          Range(0.5, 8)) = 3.0
        _FresnelStrength("Fresnel Strength",       Range(0, 3))   = 1.0

        [Header(Blending)]
        _BaseBlend      ("Base Texture Blend",     Range(0, 1))  = 1.0
        _HoloSaturation ("Holo Saturation",        Range(0, 1))  = 0.85
        _HoloBrightness ("Holo Brightness",        Range(0, 2))  = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // ---- Textures ----
            TEXTURE2D(_MainTex);      SAMPLER(sampler_MainTex);
            TEXTURE2D(_HoloMask);     SAMPLER(sampler_HoloMask);
            TEXTURE2D(_NormalMap);    SAMPLER(sampler_NormalMap);
            TEXTURE2D(_SparkleNoise); SAMPLER(sampler_SparkleNoise);

            // ---- Constant Buffer ----
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _HoloMask_ST;
                float4 _NormalMap_ST;

                float  _HoloStrength;
                float  _RainbowTiling;
                float  _ViewShift;
                float  _AnimSpeed;

                float  _StripeCount;
                float  _StripeAngle;
                float  _StripeSharpness;

                float  _SparkleStrength;
                float  _SparkleScale;
                float  _SparkleCutoff;
                float  _SparkleSharp;

                float  _FresnelPower;
                float  _FresnelStrength;

                float  _BaseBlend;
                float  _HoloSaturation;
                float  _HoloBrightness;
            CBUFFER_END

            // ---- Structs ----
            struct Attributes
            {
                float4 positionOS  : POSITION;
                float3 normalOS    : NORMAL;
                float4 tangentOS   : TANGENT;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float3 tangentWS   : TEXCOORD2;
                float3 bitangentWS : TEXCOORD3;
                float3 viewDirWS   : TEXCOORD4;
                float  fogFactor   : TEXCOORD5;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ============================================================
            //  Utility functions
            // ============================================================

            // Full HSV → RGB (hue wraps 0–1)
            float3 HsvToRgb(float h, float s, float v)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(h.xxx + K.xyz) * 6.0 - K.www);
                return v * lerp(K.xxx, saturate(p - K.xxx), s);
            }

            // Rotate a 2D UV around center (0.5, 0.5) by angle in radians
            float2 RotateUV(float2 uv, float angle)
            {
                float s, c;
                sincos(angle, s, c);
                uv -= 0.5;
                uv  = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                uv += 0.5;
                return uv;
            }

            // Smooth triangle wave (0→1→0 across 0–1 period)
            float TriWave(float x)
            {
                return 1.0 - abs(frac(x) * 2.0 - 1.0);
            }

            // ============================================================
            //  Iridescent rainbow layer
            //  Returns an RGB colour based on UV position and view angle.
            // ============================================================
            float3 IridescentRainbow(float2 uv, float3 viewDir, float3 normalWS)
            {
                // View-dependent factor: cards shift colour as you tilt them
                float vDot = dot(viewDir, normalWS);                     // 0=grazing, 1=face-on

                // Primary hue gradient — diagonal across card UV
                float hue1 = frac(
                    uv.x * 0.6 + uv.y * 0.4         // UV position contribution
                    + vDot * _ViewShift              // view angle contribution
                    + _Time.y * _AnimSpeed           // slow ambient drift
                );

                // Secondary offset layer (gives depth to the colour mixing)
                float hue2 = frac(hue1 + 0.33);     // 120° shifted for contrast
                float hue3 = frac(hue1 + 0.66);     // 240° shifted

                float3 c1 = HsvToRgb(hue1, _HoloSaturation, 1.0);
                float3 c2 = HsvToRgb(hue2, _HoloSaturation, 0.6);
                float3 c3 = HsvToRgb(hue3, _HoloSaturation, 0.4);

                // Blend by tiled luminance bands so colours flow across surface
                float band = TriWave(uv.y * _RainbowTiling + vDot * 2.0 + _Time.y * _AnimSpeed * 0.5);
                return lerp(lerp(c1, c2, band), c3, band * 0.3) * _HoloBrightness;
            }

            // ============================================================
            //  Stripe diffraction pattern
            //  Holographic foil has fine physical diffraction gratings that
            //  produce coloured stripes at specific viewing angles.
            // ============================================================
            float3 StripePattern(float2 uv, float3 viewDir, float3 normalWS)
            {
                float angleRad = radians(_StripeAngle);
                float2 rotUV   = RotateUV(uv, angleRad);

                // Tiled stripe along the rotated U axis
                float stripe = TriWave(rotUV.x * _StripeCount);
                stripe = pow(stripe, _StripeSharpness);   // sharpen the bands

                // Each stripe band gets a different hue offset
                float vDot = dot(viewDir, normalWS);
                float hue  = frac(rotUV.x * 0.8 + vDot * _ViewShift * 0.5 + _Time.y * _AnimSpeed);

                float3 stripeColor = HsvToRgb(hue, 0.9, 1.0);
                return stripeColor * stripe;
            }

            // ============================================================
            //  Sparkle / glitter
            //  Uses 3 offset noise samples multiplied together so only
            //  near-coincident bright spots survive the product → sharp glints.
            // ============================================================
            float3 Sparkle(float2 uv, float3 viewDir, float3 normalWS, float3 rainbowCol)
            {
                float2 sUV  = uv * _SparkleScale;

                float n1 = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV                         ).r;
                float n2 = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV * 0.73 + float2(0.37, 0.61)).r;
                float n3 = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV * 1.31 + float2(0.83, 0.19)).r;

                // Multiplicative combination → very sparse bright peaks
                float sparkRaw = n1 * n2 * n3;

                // Threshold and sharpen
                float sparkMask = saturate((sparkRaw - _SparkleCutoff) * _SparkleSharp);

                // View-dependent intensity — bright when near perfect reflection
                float3 reflDir     = reflect(-viewDir, normalWS);
                float  specular    = saturate(dot(reflDir, viewDir));
                float  sparkIntens = pow(specular, 6.0);

                // Glints take their hue from nearby rainbow colour, slightly shifted
                float hue          = frac(uv.x * 3.1 + uv.y * 2.7 + _Time.y * _AnimSpeed * 2.0);
                float3 sparkColor  = HsvToRgb(hue, 0.5, 1.0);   // desaturated = white-ish glint

                return sparkColor * sparkMask * sparkIntens * _SparkleStrength;
            }

            // ============================================================
            //  Fresnel rim
            //  Adds a soft rainbow halo around the card edge.
            // ============================================================
            float3 FresnelRim(float3 viewDir, float3 normalWS)
            {
                float fresnel = pow(1.0 - saturate(dot(viewDir, normalWS)), _FresnelPower);
                float hue     = frac(fresnel * 0.7 + _Time.y * _AnimSpeed * 0.3);
                float3 rimCol = HsvToRgb(hue, 0.8, 1.0);
                return rimCol * fresnel * _FresnelStrength;
            }

            // ============================================================
            //  Vertex shader
            // ============================================================
            Varyings vert(Attributes IN)
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                Varyings OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs posInputs  = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs   normInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                OUT.positionCS  = posInputs.positionCS;
                OUT.uv          = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS    = normInputs.normalWS;
                OUT.tangentWS   = normInputs.tangentWS;
                OUT.bitangentWS = normInputs.bitangentWS;
                OUT.viewDirWS   = GetWorldSpaceViewDir(posInputs.positionWS);
                OUT.fogFactor   = ComputeFogFactor(posInputs.positionCS.z);
                return OUT;
            }

            // ============================================================
            //  Fragment shader
            // ============================================================
            float4 frag(Varyings IN) : SV_Target
            {
                // --- Base card texture ---
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // --- Holo mask: R channel controls how much holo is applied ---
                float holoMask = SAMPLE_TEXTURE2D(_HoloMask, sampler_HoloMask, IN.uv).r;

                // --- Normal mapping (for micro-surface glint variation) ---
                float3 normalTS  = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
                float3x3 TBN     = float3x3(
                    normalize(IN.tangentWS),
                    normalize(IN.bitangentWS),
                    normalize(IN.normalWS)
                );
                float3 normalWS  = normalize(mul(normalTS, TBN));
                float3 viewDir   = normalize(IN.viewDirWS);

                // --- Build holo layers ---
                float3 rainbow = IridescentRainbow(IN.uv, viewDir, normalWS);
                float3 stripes = StripePattern(IN.uv, viewDir, normalWS);
                float3 sparkle = Sparkle(IN.uv, viewDir, normalWS, rainbow);
                float3 fresnel = FresnelRim(viewDir, normalWS);

                // Combine holo layers
                // Stripes modulate rainbow (like real diffraction grating over colour bands)
                float3 holo  = rainbow * (1.0 + stripes * 0.6);
                holo        += sparkle;
                holo        += fresnel;
                holo        *= _HoloStrength;

                // --- Composite: base card + holo (masked) ---
                float3 finalColor = baseColor.rgb * _BaseBlend + holo * holoMask;

                // Reinhard tonemap to prevent bloom blowout
                finalColor = finalColor / (finalColor + 1.0);

                // Apply fog
                finalColor = MixFog(finalColor, IN.fogFactor);

                return float4(finalColor, baseColor.a);
            }
            ENDHLSL
        }

        // Shadow caster pass (URP standard)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex   ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
