Shader "Custom/HeightFogCheap" {
	Properties {
		_MainTex ("Texture 1", 2D) = "white" {}
		_FogTex ("Fog Texture", 2D) = "white" {}
		_FogColour ("fog colour", Vector) = (1,1,1,1)
		_Tim ("Time", Float) = 0
		_Density ("Fog Density", Float) = 0.1
		_Plane ("_Plane", Vector) = (0,1,0,1)
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