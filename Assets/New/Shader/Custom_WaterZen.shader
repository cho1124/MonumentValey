Shader "Custom/WaterZen" {
	Properties {
		_DepthTex ("Depth Map", 2D) = "white" {}
		_ColourRamp0 ("Colour ramp 0", 2D) = "white" {}
		_LightDir ("Light Direction", Vector) = (0,-1,0,0)
		_WaveHeight ("wave height", Float) = 0.5
		_Speed ("Speed", Float) = 1
		_WaveLength ("WaveLength", Float) = 1
		_Amount ("Amount", Float) = 1
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