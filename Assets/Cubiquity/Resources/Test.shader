Shader "Custom/Test" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		struct Input
		{
			float3 worldNormal;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = half4(IN.worldNormal, 1.0);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
