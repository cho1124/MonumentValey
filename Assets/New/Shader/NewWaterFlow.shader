Shader "NewWaterFlow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Scroll ("Offset", Float) = 0
		_Scale ("_Scale", Float) = 0.25
		_ScrollU ("OffsetU", Float) = 0
		_ScaleU ("_ScaleU", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}