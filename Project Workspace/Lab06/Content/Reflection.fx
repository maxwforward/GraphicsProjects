//######################################################################################
// Effect Parameters		********** REUSED **********
//######################################################################################
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;


//######################################################################################
// Effect Parameters		********** NEW ********** (From Lab06)
//######################################################################################
texture decalMap;
sampler tsampler1 = sampler_state
{
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};


//######################################################################################
// Effect Parameters		********** NEW ********** (From Lab06 & Tutorial)
//######################################################################################
texture environmentMap; // SkyBoxTexture
samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <environmentMap>; //<SkyBoxTexture>
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};


//**************************************************************************************
// Input Data Structure
//**************************************************************************************
struct VertexShaderInput
{
	float2 texCoord: TEXCOORD0; // From Lab06 (TEXCOORD)
	float4 Position: POSITION0;
	float4 Normal: NORMAL0;
};


//**************************************************************************************
// Output Data Structure
//**************************************************************************************
struct VertexShaderOutput
{
	float3 R: TEXCOORD1; // Reflection Color
	float4 Position: POSITION0;
	float2 texCoord: TEXCOORD0; // From Lab06
};


//======================================================================================
// Vertex Shader
//======================================================================================
VertexShaderOutput ReflectionVS(VertexShaderInput input)
{
	// REUSED
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	float3 V = normalize(CameraPosition - worldPosition.xyz); // View Direction
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz); // Normal

	// NEW
	output.texCoord = input.texCoord;

	// Incident Vector (Negative View Vector)
	float3 I = (-V);
	
	// Set the Reflection Color using the Incident Vector and Normal
	output.R = reflect(I, N);
	
	return output;
}


//======================================================================================
// Pixel Shader
//======================================================================================
float4 ReflectionPS(VertexShaderOutput input) : COLOR
{
	float4 reflectColor = texCUBE(SkyBoxSampler, input.R);
	float4 decalColor = tex2D(tsampler1, input.texCoord); 
	
	//return reflectColor; // PART B
	
	// Lerp (linear interpolate) the decal map and environment map using 0.5 (50% of reflection) 
	return lerp(decalColor, reflectColor, 0.5); // PART C
}


//======================================================================================
// Technique
//======================================================================================
Technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 ReflectionVS();
		PixelShader = compile ps_4_0 ReflectionPS();
	}
}