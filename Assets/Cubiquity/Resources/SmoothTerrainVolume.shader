Shader "SmoothTerrainVolume" {
	Properties {
		_Tex0 ("Base (RGB)", 2D) = "white" {}
		_Tex1 ("Base (RGB)", 2D) = "white" {}
		_Tex2 ("Base (RGB)", 2D) = "white" {}
		//_Tex3 ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		#pragma only_renderers d3d9

		sampler2D _Tex0;
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;

		struct Input
		{
			float4 color : COLOR;
			float3 worldNormal;
			float3 worldPos;
		};
		
		half4 texTriplanar(sampler2D tex, float3 coords, float3 normal, float materialStrength)
		{
			// Squaring a normalized vector makes the components sum to one. It also seems
			// to give nicer transitions than simply dividing each compoent by the sum.
			float3 triplanarBlendWeights = normal * normal;
			
			// Modulate the contribution according to the material stength
			triplanarBlendWeights *= materialStrength;
			
			float3 dx = ddx(coords);
			float3 dy = ddy(coords);
			
			// Sample the texture three times
			half4 triplanarSample = 0.0;
			if(triplanarBlendWeights.z > 0.01)
				triplanarSample += tex2Dgrad(tex, coords.xy, dx.xy, dy.xy) * triplanarBlendWeights.z;
			if(triplanarBlendWeights.x > 0.01)
				triplanarSample += tex2Dgrad(tex, coords.yz, dx.yz, dy.yz) * triplanarBlendWeights.x;
			if(triplanarBlendWeights.y > 0.01)
				triplanarSample += tex2Dgrad(tex, coords.xz, dx.xz, dy.xz) * triplanarBlendWeights.y;
					
			return triplanarSample;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half texScale = 8.0;
			half invTexScale = 1.0 / texScale;
			
			// Interpolation can cause the normal vector to become denomalised.
			IN.worldNormal = normalize(IN.worldNormal);
			
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			//half materialStrengthsSum = materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w;
			//materialStrengths /= materialStrengthsSum;
			materialStrengths = normalize(materialStrengths);
			materialStrengths = materialStrengths * materialStrengths;
			
			
			half4 samp0 = texTriplanar(_Tex0, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz, materialStrengths.r);
			half4 samp1 = texTriplanar(_Tex1, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz, materialStrengths.g);
			half4 samp2 = texTriplanar(_Tex2, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz, materialStrengths.b);
			half4 samp3 = texTriplanar(_Tex3, IN.worldPos.xyz * invTexScale, IN.worldNormal.xyz, materialStrengths.a);
			
			half4 result = samp0 + samp1 + samp2 + samp3;
			
			//half4 c = tex2D (_Tex0, IN.uv_Tex0);
			//half c = IN.color;
			o.Albedo = result.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
