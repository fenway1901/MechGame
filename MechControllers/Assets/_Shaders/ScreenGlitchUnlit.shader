Shader "Custom/ScreenGlitchUnlit"
{
    Properties
    {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Glitch ("Glitch Amount", Range(0,1)) = 0
        _Seed ("Seed", Float) = 0

        _LineCount ("Line Count", Range(64,2048)) = 480
        _Jitter ("Line Jitter", Range(0,0.05)) = 0.01
        _RGBSplit ("RGB Split", Range(0,0.02)) = 0.004
        _Noise ("Static", Range(0,0.5)) = 0.15
        _Scanline ("Scanline", Range(0,1)) = 0.35
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _Color;

            float _Glitch;
            float _Seed;

            float _LineCount;
            float _Jitter;
            float _RGBSplit;
            float _Noise;
            float _Scanline;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                float4 color       : COLOR;
            };

            float hash11(float p)
            {
                p = frac(p * 0.1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            float hash21(float2 p)
            {
                float3 p3 = frac(float3(p.x, p.y, p.x) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y;
                float glitch = saturate(_Glitch);

                // Horizontal line-based jitter
                float lineId = floor(IN.uv.y * _LineCount);
                float rLine = hash11(lineId * 0.123 + _Seed + floor(t * 30.0));
                float jitter = (rLine - 0.5) * _Jitter * glitch;

                float2 uv = IN.uv;
                uv.x += jitter;

                // Occasional block jump (small vertical offset)
                float rBlock = hash11(lineId * 0.731 + _Seed + floor(t * 12.0));
                if (rBlock > 0.92)
                {
                    uv.y += (rBlock - 0.92) * 0.12 * glitch;
                }

                // RGB split
                float split = _RGBSplit * glitch * (0.25 + rLine);
                float2 uvR = uv + float2(split, 0);
                float2 uvG = uv;
                float2 uvB = uv - float2(split, 0);

                half4 cr = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvR);
                half4 cg = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvG);
                half4 cb = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvB);

                half4 col;
                col.rgb = half3(cr.r, cg.g, cb.b);
                col.a   = cg.a;

                // Static noise
                float n = hash21(IN.uv * (150.0 + 700.0 * hash11(_Seed + 1.7)) + t * 60.0);
                col.rgb += (n - 0.5) * (_Noise * glitch);

                // Scanlines
                float scan = sin((IN.uv.y * _LineCount) * 3.14159 + t * 80.0) * 0.5 + 0.5;
                col.rgb *= lerp(1.0, scan, _Scanline * glitch);

                col *= _Color * IN.color;
                return col;
            }
            ENDHLSL
        }
    }
}
