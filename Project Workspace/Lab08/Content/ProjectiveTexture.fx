// Variables
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4x4 LightViewMatrix;
float4x4 LightProjectionMatrix;
float3 CameraPosition;
float3 LightPosition;
float AmbientColor = 0.8f;


// Texture
texture ProjectiveTexture;


// Sampler
sampler TextureSampler = sampler_state
{
	Texture = <ProjectiveTexture>;
	MinFilter = none;
	MagFilter = none;
	MipFilter = none;
	AddressU = border;
	AddressV = border;
};


// PROJECTIVE TEXTURE INPUT
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};


// PROJECTIVE TEXTURE OUTPUT
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal	: TEXCOORD0;
	float2 TexCoord : TEXCOORD1;
	float3 WorldPosition : TEXCOORD2;
};


// SHADOW MAP INPUT
struct ShadowMapInput
{
	float4 Position: POSITION0;
};


// SHADOW MAP OUTPUT
struct ShadowMapOutput
{
	float4 Position : POSITION0;
	float4 Position2D: TEXCOORD0;
};


// PROJECTIVE TEXTURE VS
VertexShaderOutput VSFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPosition = mul(worldPos, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPos.xyz;
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.TexCoord = input.TexCoord;
	return output;
}


// PROJECTIVE TEXTURE PS
float4 PSFunction(VertexShaderOutput input) : COLOR0
{
	float4 projTexCoord = mul(mul(float4(input.WorldPosition, 1), LightViewMatrix), LightProjectionMatrix);
	projTexCoord = projTexCoord / projTexCoord.w;
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	projTexCoord.y = 1.0 - projTexCoord.y;
	float depth = 1.0 - projTexCoord.z;
	float4 color = tex2D(TextureSampler, projTexCoord.xy);
	
	//---------------------------------------- PART A -----------------------------------------//
	/*
	if (color.x == 0 && color.y == 1 && color.z == 1) color.xyz = float3(0, 0, 0);	// STEP 7
	float3 N = normalize(input.Normal);												// STEP 8
	float3 L = normalize(LightPosition - input.WorldPosition);						// STEP 8
	if (dot(L, N) < 0) color = 0;													// STEP 8
	*/
	//-----------------------------------------------------------------------------------------//
	
	return color;
	//return float4(AmbientColor, AmbientColor, AmbientColor, 1.0);
}


// SHADOW MAP VS
ShadowMapOutput ShadowMapVertexShader(ShadowMapInput input)
{ 
	ShadowMapOutput output;
	output.Position = mul(mul(input.Position, LightViewMatrix), LightProjectionMatrix); 
	output.Position2D = output.Position; 
	return output; 
}


// SHADOW MAP PS
float4 ShadowMapPixelShader(ShadowMapOutput input) : COLOR0
{
	float4 projTexCoord = input.Position;
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
	projTexCoord.y = 1.0 - projTexCoord.y;
	float depth = 1.0 - projTexCoord.z;
	float4 color = (depth > 0) ? depth : 0; 
	return color;
}


// Projective Texture Technique
technique ProjectiveTexture
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VSFunction();
		PixelShader = compile ps_4_0 PSFunction();
	}
}


// Shadow Map Technique
technique ShadowMap
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 ShadowMapVertexShader();
		PixelShader = compile ps_4_0 ShadowMapPixelShader();
	}
}