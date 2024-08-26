Shader "Custom/FresnelFade" {
	Properties {
		_Color ("_Color", Vector) = (1,0,0,1)
		_FresnelColor ("_FresnelColor", Vector) = (0,1,0,1)
		_ColorAdd ("_ColorAdd", Vector) = (0,0,0,1)
		_Blend ("_Blend", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
}