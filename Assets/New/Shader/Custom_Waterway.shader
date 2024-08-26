Shader "Custom/Waterway" {
	Properties {
		_MainTex ("_MainTex", 2D) = "white" {}
		_Opacity ("_Opacity", Float) = 0
		_FillAmount ("_FillAmount", Float) = 1
		_FlowSpeed ("_FillSpeed", Float) = 2
		_LightColour0 ("Light colour 0", Vector) = (1,0.73,0.117,0)
		_LightColour1 ("Light colour 1", Vector) = (0.05,0.275,0.275,0)
		_LightColour2 ("Light colour 2", Vector) = (0,0,0,0)
		_LightColour3 ("Rim colour", Vector) = (0,0,0,0)
		_LightTint ("Light Multiplier", Vector) = (1,1,1,0)
		_AmbientColour1 ("Light Add", Vector) = (0.5,0.5,0.5,0)
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