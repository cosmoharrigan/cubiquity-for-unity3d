Shader "ColoredCubesVolume"
{	
    SubShader
    {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert addshadow
      #pragma target 3.0
      #pragma only_renderers d3d9
      
      struct Input
      {
          float4 color : COLOR;
          float4 modelPos;
          float3 worldPos;
          float3 customColor;
      };
      
      #include "ColoredCubesVolumeUtilities.cginc"

		
		
      
      void vert (inout appdata_full v, out Input o)
      {
      	UNITY_INITIALIZE_OUTPUT(Input,o);
      	//float4 unpackedPosition = float4(unpackPosition(v.vertex.x), 1.0f);
      	//float4 unpackedColor = float4(unpackColor(v.vertex.y), 1.0f);
      	
      	//v.vertex = unpackedPosition;
      	
      	v.normal = float3 (0.0f, 0.0f, 1.0f);

    v.tangent = float4 (1.0f, 0.0f, 0.0f, 1.0f);     
          
          
          o.modelPos = v.vertex;
          
          //o.customColor = float3(0.0, v.texcoord.x, 0.0);
          //o.customColor = floatToRGB(v.texcoord.x);
          //o.customColor = unpackedColor.xyz;
      }
      
      void surf (Input IN, inout SurfaceOutput o)
      {
      	// Compute the surface normal in the fragment shader.
      	float3 surfaceNormal = normalize(cross(ddx(IN.worldPos.xyz), ddy(IN.worldPos.xyz)));
      	
	    //Add noise
	    float noise = positionBasedNoise(float4(IN.modelPos.xyz, 0.1));
        
        o.Albedo = IN.color + noise;
        o.Normal = surfaceNormal;
      }
      ENDCG
    }
    Fallback "Diffuse"
  }
  