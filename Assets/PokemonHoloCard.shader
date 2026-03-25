// ============================================================
//  PokemonHoloCard.shader — URP Forward Lit + UI Compatible
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

        [Header(UI Holo Tilt)]
        _CardTiltX      ("Card Tilt X (UI)",       Range(-1, 1)) = 0.0
        _CardTiltY      ("Card Tilt Y (UI)",       Range(-1, 1)) = 0.0

        [HideInInspector] _StencilComp     ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil         ("Stencil ID",         Float) = 0
        [HideInInspector] _StencilOp       ("Stencil Operation",  Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask",  Float) = 255
        [HideInInspector] _ColorMask       ("Color Mask",         Float) = 15
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent"
        }

        // ============================================================
        //  SHARED CODE
        //  NOTE: Structs are named HoloAttributes / HoloVaryings to
        //  avoid redefinition conflicts with ShadowCasterPass.hlsl
        //  which defines its own 'Attributes' struct at line 16.
        // ============================================================
        HLSLINCLUDE

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

            float  _CardTiltX;
            float  _CardTiltY;

            float  _StencilComp;
            float  _Stencil;
            float  _StencilOp;
            float  _StencilWriteMask;
            float  _StencilReadMask;
            float  _ColorMask;
        CBUFFER_END

        // ---- Structs — uniquely named to avoid shadow pass conflicts ----
        struct HoloAttributes
        {
            float4 positionOS  : POSITION;
            float3 normalOS    : NORMAL;
            float4 tangentOS   : TANGENT;
            float2 uv          : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct HoloVaryings
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

        float3 HsvToRgb(float h, float s, float v)
        {
            float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 p = abs(frac(h.xxx + K.xyz) * 6.0 - K.www);
            return v * lerp(K.xxx, saturate(p - K.xxx), s);
        }

        float2 RotateUV(float2 uv, float angle)
        {
            float s, c;
            sincos(angle, s, c);
            uv -= 0.5;
            uv  = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
            uv += 0.5;
            return uv;
        }

        float TriWave(float x)
        {
            return 1.0 - abs(frac(x) * 2.0 - 1.0);
        }

        // ============================================================
        //  Holo layer functions
        // ============================================================

        float3 IridescentRainbow(float2 uv, float3 viewDir, float3 normalWS)
        {
            float vDot = dot(viewDir, normalWS);
            float hue1 = frac(uv.x * 0.6 + uv.y * 0.4 + vDot * _ViewShift + _Time.y * _AnimSpeed);
            float hue2 = frac(hue1 + 0.33);
            float hue3 = frac(hue1 + 0.66);

            float3 c1 = HsvToRgb(hue1, _HoloSaturation, 1.0);
            float3 c2 = HsvToRgb(hue2, _HoloSaturation, 0.6);
            float3 c3 = HsvToRgb(hue3, _HoloSaturation, 0.4);

            float band = TriWave(uv.y * _RainbowTiling + vDot * 2.0 + _Time.y * _AnimSpeed * 0.5);
            return lerp(lerp(c1, c2, band), c3, band * 0.3) * _HoloBrightness;
        }

        float3 StripePattern(float2 uv, float3 viewDir, float3 normalWS)
        {
            float  angleRad  = radians(_StripeAngle);
            float2 rotUV     = RotateUV(uv, angleRad);
            float  stripe    = pow(TriWave(rotUV.x * _StripeCount), _StripeSharpness);
            float  vDot      = dot(viewDir, normalWS);
            float  hue       = frac(rotUV.x * 0.8 + vDot * _ViewShift * 0.5 + _Time.y * _AnimSpeed);
            return HsvToRgb(hue, 0.9, 1.0) * stripe;
        }

        float3 Sparkle(float2 uv, float3 viewDir, float3 normalWS)
        {
            float2 sUV = uv * _SparkleScale;
            float  n1  = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV).r;
            float  n2  = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV * 0.73 + float2(0.37, 0.61)).r;
            float  n3  = SAMPLE_TEXTURE2D(_SparkleNoise, sampler_SparkleNoise, sUV * 1.31 + float2(0.83, 0.19)).r;

            float  sparkMask   = saturate((n1 * n2 * n3 - _SparkleCutoff) * _SparkleSharp);
            float3 reflDir     = reflect(-viewDir, normalWS);
            float  sparkIntens = pow(saturate(dot(reflDir, viewDir)), 6.0);
            float  hue         = frac(uv.x * 3.1 + uv.y * 2.7 + _Time.y * _AnimSpeed * 2.0);
            return HsvToRgb(hue, 0.5, 1.0) * sparkMask * sparkIntens * _SparkleStrength;
        }

        float3 FresnelRim(float3 viewDir, float3 normalWS)
        {
            float  fresnel = pow(1.0 - saturate(dot(viewDir, normalWS)), _FresnelPower);
            float  hue     = frac(fresnel * 0.7 + _Time.y * _AnimSpeed * 0.3);
            return HsvToRgb(hue, 0.8, 1.0) * fresnel * _FresnelStrength;
        }

        // ============================================================
        //  Shared composite helper
        // ============================================================
        float4 ComputeHoloFragment(HoloVaryings IN, float3 viewDir)
        {
            float4 baseColor = SAMPLE_TEXTURE2D(_MainTex,  sampler_MainTex,  IN.uv);
            float  holoMask  = SAMPLE_TEXTURE2D(_HoloMask, sampler_HoloMask, IN.uv).r;

            float3 normalTS  = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv));
            float3x3 TBN     = float3x3(
                normalize(IN.tangentWS),
                normalize(IN.bitangentWS),
                normalize(IN.normalWS)
            );
            float3 normalWS  = normalize(mul(normalTS, TBN));

            float3 rainbow = IridescentRainbow(IN.uv, viewDir, normalWS);
            float3 stripes = StripePattern(IN.uv, viewDir, normalWS);
            float3 sparkle = Sparkle(IN.uv, viewDir, normalWS);
            float3 fresnel = FresnelRim(viewDir, normalWS);

            float3 holo  = rainbow * (1.0 + stripes * 0.6) + sparkle + fresnel;
            holo        *= _HoloStrength;

            float3 final = baseColor.rgb * _BaseBlend + holo * holoMask;
            final        = final / (final + 1.0);   // Reinhard tonemap

            return float4(final, baseColor.a);
        }

        // ============================================================
        //  Shared vertex shader
        // ============================================================
        HoloVaryings vert(HoloAttributes IN)
        {
            UNITY_SETUP_INSTANCE_ID(IN);
            HoloVaryings OUT;
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

        ENDHLSL

        // ============================================================
        //  PASS 1 — 3D forward lit
        // ============================================================
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

            float4 frag(HoloVaryings IN) : SV_Target
            {
                float3 viewDir    = normalize(IN.viewDirWS);
                float4 finalColor = ComputeHoloFragment(IN, viewDir);
                finalColor.rgb    = MixFog(finalColor.rgb, IN.fogFactor);
                return finalColor;
            }
            ENDHLSL
        }

        // ============================================================
        //  PASS 2 — UI Canvas
        // ============================================================
        Pass
        {
            Name "UIHolo"
            Tags { }

            Cull Off
            ZWrite Off
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask [_ColorMask]

            Stencil
            {
                Ref       [_Stencil]
                Comp      [_StencilComp]
                Pass      [_StencilOp]
                ReadMask  [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment fragUI

            float4 fragUI(HoloVaryings IN) : SV_Target
            {
                // Fake view direction driven by script-supplied tilt values
                float3 viewDir = normalize(float3(_CardTiltX, _CardTiltY, 1.0));
                return ComputeHoloFragment(IN, viewDir);
            }
            ENDHLSL
        }

        // ============================================================
        //  PASS 3 — Shadow caster
        //  Isolated in its own HLSLPROGRAM so HLSLINCLUDE structs
        //  (HoloAttributes / HoloVaryings) are compiled separately
        //  and never clash with ShadowCasterPass.hlsl's own Attributes.
        // ============================================================
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex   ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_shadowcaster

            // Must come AFTER the pragmas — this file defines its own
            // Attributes and Varyings structs internally.
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}