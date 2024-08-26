Shader "Custom/CharacterFlatHeightFog" {
	Properties {
		_Colour ("Colour", Vector) = (1,1,1,1)
		_FogTex ("Fog Texture", 2D) = "white" {}
		_FogColour ("fog colour", Vector) = (1,1,1,1)
		_Tim ("Time", Float) = 0
		_Density ("Fog Density", Float) = 0.1
		_Plane ("_Plane", Vector) = (0,1,0,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}