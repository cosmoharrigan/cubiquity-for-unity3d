Shader "SmoothTerrainVolume" {
	Properties {
		_Tex0 ("Base (RGB)", 2D) = "white" {}
		_Tex1 ("Base (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma only_renderers d3d9

		sampler2D _Tex0;
		sampler2D _Tex1;

		struct Input {
			//float2 uv_Tex0;
			float4 color : COLOR;
			float3 modelPos;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			v.texcoord  = v.vertex;
			
			o.modelPos = v.vertex;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 samp0 = tex2D(_Tex0, IN.modelPos.xy);
			half4 samp1 = tex2D(_Tex1, IN.modelPos.xy);
			
			half4 result = samp0 * IN.color.r + samp1 * IN.color.g;
			//half4 c = tex2D (_Tex0, IN.uv_Tex0);
			//half c = IN.color;
			o.Albedo = result.rgb;
			o.Alpha = 1.0f;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
