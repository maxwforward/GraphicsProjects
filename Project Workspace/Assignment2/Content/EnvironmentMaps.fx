//#############################################################################################################################################
//  GLOBAL VARIABLES                                                                                                           (FROM LAB06)
//#############################################################################################################################################
float4x4 World; 
float4x4 View; 
float4x4 Projection; 
float4x4 WorldInverseTranspose; 
float3 CameraPosition; 
texture decalMap; 
texture environmentMap;


//#############################################################################################################################################
//  GLOBAL VARIABLES                                                                                                          
//#############################################################################################################################################
float Reflectivity;
float3 etaRatio;
float FresnelBias;
float FresnelScale;
float FresnelPower;


//#############################################################################################################################################
//  TEXTURE SAMPLER                                                                                                            (FROM LAB06)
//#############################################################################################################################################
sampler tsampler1 = sampler_state 
{ 
	texture = <decalMap>;
	/*magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;*/
}; 


//#############################################################################################################################################
//  CUBE SAMPLER                                                                                                               (FROM LAB06)
//#############################################################################################################################################
samplerCUBE SkyBoxSampler = sampler_state 
{ 
	texture = <environmentMap>;
	/*magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;*/
}; 


//#############################################################################################################################################
//  INPUT DATA STRUCTURE                                                                                                       (FROM LAB06)
//#############################################################################################################################################
struct VertexShaderInput 
{ 
	float4 Position: POSITION0; 
	float2 texCoord: TEXCOORD0; 
	float4 normal: NORMAL0; 
}; 


//#############################################################################################################################################
//  OUTPUT DATA STRUCTURE                                                                                                      (FROM LAB06)
//#############################################################################################################################################
struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float2 texCoord: TEXCOORD0;
	//float3 R: TEXCOORD1;				// Per-Vertex (Reflection)
	float4 WorldPosition : TEXCOORD1;	// Per-Pixel
	float4 Normal : TEXCOORD2;			// Per-Pixel
};


//=============================================================================================================================================
//  VERTEX SHADER (PER-PIXEL)                                                                                                  (FROM LAB06)
//=============================================================================================================================================
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 projPosition = mul(viewPosition, Projection);
	output.Position = projPosition;
	//float3 N = normalize(mul(input.normal, WorldInverseTranspose).xyz);	// Per-Vertex
	//float3 I = normalize(worldPosition.xyz - CameraPosition);				// Per-Vertex
	//output.R = reflect(I, N);												// Per-Vertex (Reflection)
	output.WorldPosition = worldPosition;									// Per-Pixel
	output.Normal = normalize(mul(input.normal, WorldInverseTranspose));	// Per-Pixel
	output.texCoord = input.texCoord;
	return output;
}


//=============================================================================================================================================
//  REFLECTION PIXEL SHADER (PER-PIXEL)                                                                                        (FROM LAB06)
//=============================================================================================================================================
float4 ReflectionPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = normalize(input.Normal.xyz);							// Per-Pixel
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);	// Per-Pixel
	float3 I = -(V);												// Per-Pixel
	float3 R = reflect(I, N);										// Per-Pixel
	float4 reflectedColor = texCUBE(SkyBoxSampler, R);
	float4 decalColor = tex2D(tsampler1, input.texCoord);
	return lerp(decalColor, reflectedColor, Reflectivity);
}


//=============================================================================================================================================
//  REFRACTION PIXEL SHADER (PER-PIXEL)
//=============================================================================================================================================
float4 RefractionPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 I = -(V);
	float3 R = refract(I, N, etaRatio.x);
	float4 refractedColor = texCUBE(SkyBoxSampler, R);
	float4 decalColor = tex2D(tsampler1, input.texCoord);
	return lerp(decalColor, refractedColor, Reflectivity);
}


//=============================================================================================================================================
//  REFRACTION + DISPERSION PIXEL SHADER (PER-PIXEL)
//=============================================================================================================================================
float4 DispersionPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 I = -(V);
	float3 Red = refract(I, N, etaRatio.x);
	float3 Green = refract(I, N, etaRatio.y);
	float3 Blue = refract(I, N, etaRatio.z);
	float4 refractedColor;
	refractedColor.r = texCUBE(SkyBoxSampler, Red).r;
	refractedColor.g = texCUBE(SkyBoxSampler, Green).g;
	refractedColor.b = texCUBE(SkyBoxSampler, Blue).b;
	refractedColor.a = 1;
	float4 decalColor = tex2D(tsampler1, input.texCoord);
	return lerp(decalColor, refractedColor, Reflectivity);
}


//=============================================================================================================================================
//  FRESNEL PIXEL SHADER (PER-PIXEL)
//=============================================================================================================================================
float4 FresnelPixelShaderFunction(VertexShaderOutput input) : COLOR0 // This combines reflection, refraction, dispersion, and fresnel
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 I = -(V);
	float3 R = reflect(I, N); // Reflection vector
	float3 Red = refract(I, N, etaRatio.x);
	float3 Green = refract(I, N, etaRatio.y);
	float3 Blue = refract(I, N, etaRatio.z);
	float reflectionFactor = max(0, min(1, (FresnelBias + FresnelScale * pow(1 + dot(I, N), FresnelPower))));
	float4 reflectedColor = texCUBE(SkyBoxSampler, R);
	float4 refractedColor;
	refractedColor.r = texCUBE(SkyBoxSampler, Red).r;
	refractedColor.g = texCUBE(SkyBoxSampler, Green).g;
	refractedColor.b = texCUBE(SkyBoxSampler, Blue).b;
	refractedColor.a = 1;
	float4 decalColor = tex2D(tsampler1, input.texCoord);
	float4 fresnelColor = lerp(reflectedColor, refractedColor, reflectionFactor);
	return lerp(decalColor, fresnelColor, Reflectivity);
}


//=============================================================================================================================================
//  REFLECTION TECHNIQUE BLOCK                                                                                           (FROM ASSIGNMENT1)
//=============================================================================================================================================
technique Reflection 
{
	pass Pass1 
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 ReflectionPixelShaderFunction();
	}
}


//=============================================================================================================================================
//  REFRACTION TECHNIQUE BLOCK                                                                                           (FROM ASSIGNMENT1)
//=============================================================================================================================================
technique Refraction
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 RefractionPixelShaderFunction();
	}
}


//=============================================================================================================================================
//  REFRACTION + DISPERSION TECHNIQUE BLOCK                                                                              (FROM ASSIGNMENT1)
//=============================================================================================================================================
technique Dispersion
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 DispersionPixelShaderFunction();
	}
}


//=============================================================================================================================================
//  FRESNEL TECHNIQUE BLOCK                                                                                              (FROM ASSIGNMENT1)
//=============================================================================================================================================
technique Fresnel
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 FresnelPixelShaderFunction();
	}
}