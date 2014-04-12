Shader "ColoredCubesVolume"
{	
    SubShader
    {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert addshadow
      #pragma target 3.0
      #pragma glsl
      
      // Scripts can flip the computed normal by setting this to '-1.0f'.
      float normalMultiplier;
      
      float4x4 _World2Volume;
      
      struct Input
      {
          float4 color : COLOR;
          float4 modelPos;
          float4 volumePos;
      };
      
      #include "ColoredCubesVolumeUtilities.cginc"

		
		
      
      void vert (inout appdata_full v, out Input o)
      {
      	UNITY_INITIALIZE_OUTPUT(Input,o);
      	
      	// Unity can't cope with the idea that we're peforming lighting without having per-vertex
      	// normals. We specify dummy ones here to avoid having to use up vertex buffer space for them.
      	v.normal = float3 (0.0f, 0.0f, 1.0f);
      	v.tangent = float4 (1.0f, 0.0f, 0.0f, 1.0f);     
        
        // Model-space position is use for adding noise.
        o.modelPos = v.vertex;
        float4 worldPos = mul(_Object2World, v.vertex);
        o.volumePos =  mul(_World2Volume, worldPos);
      }
      
      void surf (Input IN, inout SurfaceOutput o)
      {
      	// Compute the surface normal in the fragment shader. I believe the orientation of the vector is different
      	// between render systems as some are left-handed and some are right handed (even though Unity corrects for
      	// other handiness differences internally). With these render system tests the normals are correct on Windows
      	// regardless of which render system is in use.
#if SHADER_API_D3D9 || SHADER_API_D3D11 || SHADER_API_D3D11_9X || SHADER_API_XBOX360
      	float3 surfaceNormal = normalize(cross(ddx(IN.modelPos.xyz), ddy(IN.modelPos.xyz)));
#else
		float3 surfaceNormal = -normalize(cross(ddx(IN.modelPos.xyz), ddy(IN.modelPos.xyz)));
#endif

		// Despite our render system checks above, we have seen that the normals are still backwards in Linux
      	// standalone builds. The reason is not currently clear, but the 'normalMultiplier' allow scripts to
      	// flip the normal if required by setting the multiplier to '-1.0f'.
		surfaceNormal *= normalMultiplier;
      	
	    //Add noise - we use model space to prevent noise scrolling if the volume moves.
	    float noise = positionBasedNoise(float4(IN.volumePos.xyz, 0.1));
        
        o.Albedo = IN.color.xyz + float3(noise, noise, noise);
        o.Normal = surfaceNormal;
      }
      ENDCG
    }
  }
  