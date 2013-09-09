Shader "SmoothTerrainVolume" {
	Properties {
		_Tex0 ("Base (RGB)", 2D) = "white" {}
		_Tex1 ("Base (RGB)", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma exclude_renderers flash 

		sampler2D _Tex0;
		sampler2D _Tex1;

		struct Input
		{
			float4 color : COLOR;
			float3 worldNormal;
			float3 worldPos;
		};
		
		half4 texTriplanar(sampler2D tex, float3 coords, float3 norm)
		{
			// Squaring a normalized vector makes the components sum to one.
			float3 blendWeights = norm * norm;
			
			// Sample the texture three times
			half4 sampXY = tex2D(tex, coords.xy);
			half4 sampYZ = tex2D(tex, coords.yz);
			half4 sampXZ = tex2D(tex, coords.xz);
			
			// Blend the samples
			return (sampXY * blendWeights.z + sampYZ * blendWeights.x + sampXZ * blendWeights.y);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half texScale = 8.0f;
			half invTexScale = 1.0f / texScale;
			half4 samp0 = texTriplanar(_Tex0, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz);
			half4 samp1 = texTriplanar(_Tex1, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz);
			
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
