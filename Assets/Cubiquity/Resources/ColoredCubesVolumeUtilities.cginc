#ifndef COLORED_CUBES_VOLUME_UTILITIES
#define COLORED_CUBES_VOLUME_UTILITIES

float3 unpackPosition(float packedPosition)
{	
	// Store the input in each component
	float3 packedVec = float3(packedPosition, packedPosition, packedPosition);
	
	// Convert each component to a value in the range 0-255      	
	float3 result = floor(packedVec / float3(65536.0, 256.0, 1.0));	      	
	float3 shiftedResult = float3(0.0, result.rg) * 256.0;	
	result -= shiftedResult;
		
	result -= float3(0.5, 0.5, 0.5);
		
	// Return the result	
	return result;
}
      
float3 unpackColor(float inputVal)
{	
	// Store the input in each component
	float3 inputVec = float3(inputVal, inputVal, inputVal);
		
	// Convert each component to a value in the range 0-255      	
	float3 result = floor(inputVec / float3(256.0, 16.0, 1.0));	      	
	float3 shiftedResult = float3(0.0, result.rg) * 16.0;	
	result -= shiftedResult;
	
	// Convert to range 0-1
	result /= 15.0;
		
	// Return the result	
	return result;
}

float positionBasedNoise(float4 positionAndStrength)
{
	//'floor' is more widely supported than 'round'. Offset consists of:
	//  - An integer to push us away from the origin (divide by zero causes a ringing artifact
	//    at one point in the world, and we want to pushthis somewhere it won't be seen.)
	//  - 0.5 to perform the rounding
	//  - A tiny offset to prevent sparkes as faces are exactly on rounding boundary.
	float3 roundedPos = floor(positionAndStrength.xyz + vec3(1000.501));
	
	//Our noise function generate banding for high inputs, so wrap them
	roundedPos = fmod(roundedPos, float3(17.0, 19.0, 23.0));
	
	//Large number is arbitrary, but smaller number lead to banding. '+ 1.0' prevents divide-by-zero
	float noise = 100000000.0 / (dot(roundedPos, roundedPos) + 1.0);
	noise = fract(noise);
	
	//Scale the noise
	float halfNoiseStrength = positionAndStrength.w * 0.5;
	noise = -halfNoiseStrength + positionAndStrength.w * noise; //http://www.opengl.org/wiki/GLSL_Optimizations#Get_MAD
	
	return noise;
}
		
#endif //COLORED_CUBES_VOLUME_UTILITIES