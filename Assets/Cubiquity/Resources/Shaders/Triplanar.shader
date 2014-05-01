Shader "Triplanar"
{
	Properties {
		_Tex0 ("Base (RGB)", 2D) = "white" {}
		_Tex1 ("Base (RGB)", 2D) = "white" {}
		_Tex2 ("Base (RGB)", 2D) = "white" {}
		_Tex3 ("Base (RGB)", 2D) = "white" {}
		_Tex4 ("Base (RGB)", 2D) = "white" {}
		_Tex5 ("Base (RGB)", 2D) = "white" {}
		_Tex6 ("Base (RGB)", 2D) = "white" {}
		_Tex7 ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert addshadow
		#pragma target 3.0
		#pragma glsl
		#pragma multi_compile BRUSH_MARKER_OFF BRUSH_MARKER_ON
		
		#include "TerrainVolumeUtilities.cginc"

		sampler2D _Tex0;
		sampler2D _Tex1;
		sampler2D _Tex2;
		sampler2D _Tex3;
		sampler2D _Tex4;
		sampler2D _Tex5;
		sampler2D _Tex6;
		sampler2D _Tex7;
		
		float4 _Tex0_ST;
		float4 _Tex1_ST;
		float4 _Tex2_ST;
		float4 _Tex3_ST;
		float4 _Tex4_ST;
		float4 _Tex5_ST;
		float4 _Tex6_ST;
		float4 _Tex7_ST;
		
		//float3 _TexInvScale0;
		//float3 _TexInvScale1;
		//float3 _TexInvScale2;
		//float3 _TexInvScale3;
		
		//float3 _TexOffset0;
		//float3 _TexOffset1;
		//float3 _TexOffset2;
		//float3 _TexOffset3;
		
#if BRUSH_MARKER_ON
		float4 BrushCenter;
		float4 BrushSettings;
		float4 BrushColor;
#endif

		float4x4 _World2Volume;

		struct Input
		{
			float4 color : COLOR;
			float4 otherFourMatStrengths : TEXCOORD0;
			float3 worldPos : POSITION;
			float3 volumeNormal;
			float4 volumePos;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			// Volume-space positions and normals are used for triplanar texturing
			float4 worldPos = mul(_Object2World, v.vertex);
			o.volumePos =  mul(_World2Volume, worldPos);
			o.volumeNormal = v.normal;
			
			// The first four material weights are stored in color and are copied by Unity.
			// But I don't think it copied the tangents (it can't know to) so do that here.
			//o.otherFourMatStrengths = v.tangent;
			o.otherFourMatStrengths.xy = v.texcoord.xy;
			o.otherFourMatStrengths.zw = v.texcoord1.xy;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			//half texScale = 8.0;
			//half invTexScale = 1.0 / texScale;
			
			// Interpolation can cause the normal vector to become denomalised.
			IN.volumeNormal = normalize(IN.volumeNormal);
			
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			half4 otherFourMaterialStrengths = IN.otherFourMatStrengths;
			half materialStrengthsSum = 
				materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w +
				otherFourMaterialStrengths.x + otherFourMaterialStrengths.y + otherFourMaterialStrengths.z + otherFourMaterialStrengths.w;
			materialStrengths /= materialStrengthsSum;
			otherFourMaterialStrengths /= materialStrengthsSum;
			
			// Texture coordinates are calculated from the model
			// space position, scaled by a user-supplied factor.
			float3 texCoords = IN.volumePos.xyz; // * invTexScale;
			
			// Texture coordinate derivatives are explicitly calculated
			// so that we can sample textures inside conditional logic.
			float3 dx = ddx(texCoords);
			float3 dy = ddy(texCoords);
			
			// Squaring a normalized vector makes the components sum to one. It also seems
			// to give nicer transitions than simply dividing each component by the sum.
			float3 triplanarBlendWeights = IN.volumeNormal * IN.volumeNormal;	
			
			// Sample each of the four textures using triplanar texturing, and
			// additively blend the results using the factors in materialStrengths.
			half4 diffuse = 0.0;
			diffuse += texTriplanar(_Tex0, texCoords, _Tex0_ST, dx, dy, triplanarBlendWeights * materialStrengths.r);
			diffuse += texTriplanar(_Tex1, texCoords, _Tex1_ST, dx, dy, triplanarBlendWeights * materialStrengths.g);
			diffuse += texTriplanar(_Tex2, texCoords, _Tex2_ST, dx, dy, triplanarBlendWeights * materialStrengths.b);
			diffuse += texTriplanar(_Tex3, texCoords, _Tex3_ST, dx, dy, triplanarBlendWeights * materialStrengths.a);
			
			diffuse += texTriplanar(_Tex4, texCoords, _Tex4_ST, dx, dy, triplanarBlendWeights * otherFourMaterialStrengths.r);
			diffuse += texTriplanar(_Tex5, texCoords, _Tex5_ST, dx, dy, triplanarBlendWeights * otherFourMaterialStrengths.g);
			diffuse += texTriplanar(_Tex6, texCoords, _Tex6_ST, dx, dy, triplanarBlendWeights * otherFourMaterialStrengths.b);
			diffuse += texTriplanar(_Tex7, texCoords, _Tex7_ST, dx, dy, triplanarBlendWeights * otherFourMaterialStrengths.a);
			
#if BRUSH_MARKER_ON
			float brushStrength = 0.0f;
			
			float distToBrushCenter = length(IN.volumePos.xyz - BrushCenter.xyz);
			if(distToBrushCenter < BrushSettings.x)
			{
				brushStrength = 1.0;
			}
			else if(distToBrushCenter < BrushSettings.y)
			{
				float lerpFactor = (distToBrushCenter - BrushSettings.x) / (BrushSettings.y - BrushSettings.x);
				brushStrength = lerp(1.0f, 0.0f, lerpFactor);
		
				brushStrength = min(brushStrength, 1.0f);
				brushStrength = max(brushStrength, 0.0f);
				
				//brushStrength = 1.0 - lerpFactor;
			}
			
			BrushColor.a = BrushColor.a * brushStrength;
			
			o.Albedo = diffuse.rgb * (1.0 - BrushColor.a) + BrushColor.rgb * BrushColor.a;
#else
			o.Albedo = diffuse.rgb;
#endif

			o.Alpha = 1.0;
		}
		ENDCG
	} 
}
