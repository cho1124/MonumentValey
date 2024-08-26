Shader "Custom/DiffuseNoLightMap" {
	Properties {
		_MainTex ("Texture 1", 2D) = "white" {}
		_Light0 ("Light0", Vector) = (0.5,-1,0.5,0)
		_Light1 ("Light1", Vector) = (0,0,0.1,0)
		_Light2 ("Light2", Vector) = (0.15,0,0.15,0)
		_LightColour0 ("Light colour 0", Vector) = (1,0.73,0.117,0)
		_LightColour1 ("Light colour 1", Vector) = (0.05,0.275,0.275,0)
		_LightColour2 ("Light colour 2", Vector) = (0,0,0,0)
		_AmbientColour1 ("Ambient Light colour", Vector) = (0.5,0.5,0.5,0)
		_LightTint ("Light Tint", Vector) = (1,1,1,0)
		_UseLightMap ("Use light map", Float) = 1
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