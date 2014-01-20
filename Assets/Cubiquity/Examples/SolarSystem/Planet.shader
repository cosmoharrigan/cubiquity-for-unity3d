Shader "Planet"
{
	Properties
	{
		_Tex0 ("Base (RGB)", CUBE) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert addshadow
		#pragma target 3.0
		#pragma only_renderers d3d9
		
		samplerCUBE _Tex0;
		
		float4x4 _World2Volume;

		struct Input
		{
			float4 color : COLOR;
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
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			IN.volumeNormal = normalize(IN.volumeNormal);
			
			// Unity's cubemap functionality is intended for reflection maps rather than wrapping 
			// textures around a sphere. As a result they show up backwards uness we invert them here.
			IN.volumeNormal.x = -IN.volumeNormal.x;
			
			// Vertex colors coming out of Cubiquity don't actually sum to one
			// (roughly 0.5 as that's where the isosurface is). Make them sum
			// to one, though Cubiquity should probably be changed to do this.
			half4 materialStrengths = IN.color;
			half materialStrengthsSum = materialStrengths.x + materialStrengths.y + materialStrengths.z + materialStrengths.w;
			materialStrengths /= materialStrengthsSum;
			
			float theta = atan(IN.volumeNormal.z/IN.volumeNormal.x);
			
			float x = theta / 3.14159265359;
			float y = IN.volumeNormal.y;
			
			
			o.Albedo  = texCUBE(_Tex0, IN.volumeNormal);
			//o.Albedo = float3(x, 0.0, 0.0);

			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
