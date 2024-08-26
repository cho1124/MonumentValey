Shader "Custom/Crush Water Tile" {
	Properties {
		_MainTex ("Texture 1", 2D) = "white" {}
		_Colour0 ("Colour 0", Vector) = (1,0.73,0.117,0)
		_Colour1 ("Colour 1", Vector) = (0.05,0.275,0.275,0)
		_Colour2 ("Colour 2", Vector) = (1,0,0,0)
		_SpeedA ("Speed A", Float) = 1
		_SpeedB ("Speed B", Float) = 1
		_FogColour ("fog colour", Vector) = (1,1,1,1)
		_Density ("Fog Density", Float) = 0.1
		_Plane ("_Plane", Vector) = (0,1,0,1)
		_ClipPlane ("_ClipPlane", Vector) = (1,0,0,1)
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