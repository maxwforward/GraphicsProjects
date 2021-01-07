﻿//########################################################################################################################################
//  LAB10 - Variables
//########################################################################################################################################
float4x4 World; 
float4x4 View; 
float4x4 Projection; 
float4x4 InverseCamera; // Inverse Camera Matrix
texture2D Texture; 


//########################################################################################################################################
//  LAB10 - Particle Sampler
//########################################################################################################################################
sampler ParticleSampler: register(s0) = sampler_state
{
	Texture = <Texture>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


//########################################################################################################################################
//  LAB10 - Input Data Structure
//########################################################################################################################################
struct VertexShaderInput
{ 
	float4 Position: POSITION; 
	float2 TexCoord: TEXCOORD0; 
	float4 ParticlePosition: POSITION1; 
	float4 ParticleParamater: POSITION2; // x: Scale x/y: Color
}; 


//########################################################################################################################################
//  LAB10 - Output Data Structure
//########################################################################################################################################
struct VertexShaderOutput 
{
	float4 Position: POSITION; 
	float2 TexCoord: TEXCOORD0; 
	float4 Color: COLOR0; 
};


//########################################################################################################################################
//  LAB10 - Vertex Shader
//########################################################################################################################################
VertexShaderOutput ParticleVertexShader(VertexShaderInput input) 
{ 
	VertexShaderOutput output; 
	float4 worldPosition = mul(input.Position, InverseCamera); 
	worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParamater.x); 
	worldPosition += input.ParticlePosition; 
	output.Position = mul(mul(mul(worldPosition, World), View), Projection); 
	output.TexCoord = input.TexCoord; 
	output.Color = 1 - input.ParticleParamater.x / input.ParticleParamater.y; 
	return output; 
} 


//########################################################################################################################################
//  LAB10 - Pixel Shader
//########################################################################################################################################
float4 ParticlePixelShader(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ParticleSampler, input.TexCoord); 
	color *= input.Color;
	return color;
}


//########################################################################################################################################
//  LAB10 - Technique Block
//########################################################################################################################################
technique particle
{
	pass P0
	{
		VertexShader = compile vs_4_0 ParticleVertexShader();
		PixelShader = compile ps_4_0 ParticlePixelShader();
	}
}