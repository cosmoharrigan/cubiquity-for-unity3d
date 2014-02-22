Shader "Custom/Test"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		struct Input
		{
			float3 customColor;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.customColor = abs(v.normal);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = half4(IN.customColor, 1.0);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
