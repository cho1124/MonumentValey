Shader "Custom/URP_DepthMask"
{
    Properties
    {
        _ZOffset ("ZOffset", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            Name "DepthMask"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            CBUFFER_START(UnityPerMaterial)
                float _ZOffset;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                float4 positionWS = mul(unity_ObjectToWorld, v.positionOS);
                positionWS.z += _ZOffset;
                o.positionCS = mul(UNITY_MATRIX_VP, positionWS);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.positionCS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                return half4(1, 1, 1, 1); // Albedo °ª 1
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
