Shader "Vert/GrowTreeLeavesDoubleClipCompound" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Magnitude ("Distortion Magnitude", Float) = 1
		_Frequency ("Distortion Frequency", Float) = 1
		_Wavelength ("Distortion Wavelength", Float) = 1
		_Plane ("_Plane", Vector) = (0,1,0,1)
		_Plane2 ("_Plane2", Vector) = (0,1,0,1)
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