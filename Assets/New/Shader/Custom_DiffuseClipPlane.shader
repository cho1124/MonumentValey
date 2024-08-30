Shader "Custom/URP_DiffuseClipPlane"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _LightColour0 ("Light colour 0", Vector) = (1,0.73,0.117,0)
        _LightColour1 ("Light colour 1", Vector) = (0.05,0.275,0.275,0)
        _LightColour2 ("Light colour 2", Vector) = (0,0,0,0)
        _LightColour3 ("Rim colour", Vector) = (0,0,0,0)
        _LightTint ("Light Multiplier", Vector) = (1,1,1,0)
        _AmbientColour1 ("Light Add", Vector) = (0.5,0.5,0.5,0)
        _ShadowColour ("Shadow Tint", Vector) = (0,0,0,0)
        _ShadowRamp ("ShadowBoost", Float) = 1
        _UseLightMap ("Debug Mode", Float) = 1
        _Plane ("_Plane", Vector) = (0,1,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
        Pass
        {
            Name "MainPass"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            sampler2D _MainTex;
            float4 _Plane;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                UNITY_FOG_COORDS(1)
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
                o.positionCS = mul(UNITY_MATRIX_VP, float4(o.positionWS, 1.0));
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o, o.positionCS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // 클리핑 평면에 대한 계산
                float distance = dot(float4(i.positionWS, 1.0), _Plane);
                clip(distance); // 클리핑 실행

                // 텍스처를 샘플링하여 Albedo 및 Alpha 설정
                half4 c = tex2D(_MainTex, i.uv);
                return half4(c.rgb, c.a);
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/InternalErrorShader"
}
