Shader "Custom/WaterwayCascade" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Opacity ("_Opacity", Float) = 0
		_FillAmount ("_FillAmount", Float) = 1
		_FlowSpeed ("_FillSpeed", Float) = 2
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
}