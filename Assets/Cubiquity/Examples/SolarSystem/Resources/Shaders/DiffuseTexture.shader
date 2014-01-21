// From here: http://docs.unity3d.com/Documentation/Components/SL-SurfaceShaderLightingExamples.html
Shader "DiffuseTexture"
{
    Properties
    {
    	_MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
    	Tags { "RenderType" = "Opaque" }
    	
		CGPROGRAM
		#pragma surface surf Lambert
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		}
		ENDCG
    } 
    
    Fallback "Diffuse"
  }