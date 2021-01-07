//=========================================================================================================================================
//  LAB10 - Variables
//=========================================================================================================================================
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseCamera; // Inverse Camera Matrix
texture2D Texture;


//#########################################################################################################################################
//  ASSIGNMENT4 - Variables (Without Texture)
//#########################################################################################################################################
float4x4 WorldInverseTranspose;
float3 CameraPosition;
float3 LightPosition;


//=========================================================================================================================================
//  LAB10 - Sampler
//=========================================================================================================================================
sampler ParticleSampler: register(s0) = sampler_state
{
	Texture = <Texture>;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


//=========================================================================================================================================
//  LAB10 - Input Data Structure
//=========================================================================================================================================
struct VertexShaderInput
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 ParticlePosition: POSITION1;
	float4 ParticleParamater: POSITION2;
};


//=========================================================================================================================================
//  LAB10 - Output Data Structure
//=========================================================================================================================================
struct VertexShaderOutput
{
	float4 Position: POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
};


//#########################################################################################################################################
//  ASSIGNMENT4 - Output Data Structure (Without Texture)
//#########################################################################################################################################
struct PhongVertexShaderOutput
{
	float4 Position : POSITION;
	float2 TexCoord: TEXCOORD0;
	float4 Color: COLOR0;
	
	// ASSIGNMENT1
	float4 WorldPosition : TEXCOORD1;
	float4 Normal : TEXCOORD2;
};


//=========================================================================================================================================
//  LAB10 - Vertex Shader
//=========================================================================================================================================
VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, InverseCamera); // Rotate polygon to face camera
	worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParamater.x); // Scale the polygon
	worldPosition += input.ParticlePosition; // Move the polygon
	output.Position = mul(mul(mul(worldPosition, World), View), Projection);
	output.TexCoord = input.TexCoord;
	output.Color = 1 - input.ParticleParamater.x / input.ParticleParamater.y; // Change the polygon color
	return output;
}


//#########################################################################################################################################
//  ASSIGNMENT4 - Vertex Shader (Without Texture)
//#########################################################################################################################################
PhongVertexShaderOutput ParticlePhongVertexShader(VertexShaderInput input)
{
	PhongVertexShaderOutput output;
	float4 worldPosition = mul(input.Position, InverseCamera); // Rotate polygon to face camera
	worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParamater.x); // Scale the polygon
	worldPosition += input.ParticlePosition; // Move the polygon
	output.Position = mul(mul(mul(worldPosition, World), View), Projection);
	output.TexCoord = input.TexCoord;
	output.Color = 1 - input.ParticleParamater.x / input.ParticleParamater.y;

	// ASSIGNMENT1
	output.WorldPosition = worldPosition;
	output.Normal = normalize(mul((input.Position - input.ParticlePosition), WorldInverseTranspose));
	
	return output;
}


//=========================================================================================================================================
//  LAB10 - Pixel Shader
//=========================================================================================================================================
float4 ParticlePixelShader(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(ParticleSampler, input.TexCoord);
	color *= input.Color;
	return color;
}


//#########################################################################################################################################
//  ASSIGNMENT4 - Pixel Shader (Without Texture)
//#########################################################################################################################################
float4 ParticlePhongPixelShader(PhongVertexShaderOutput input) : COLOR
{
	// ASSIGNMENT1
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 L = normalize(LightPosition);
	float3 R = normalize(dot(-L, N));
	float4 AmbientColor = float4(0.1f, 0.1f, 0.1f, 1);
	float AmbientIntensity = 0.1f;
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 DiffuseColor = float4(0, 1, 0, 1);
	float DiffuseIntensity = 1.0f;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 SpecularColor = float4(1, 1, 1, 1);
	float Shininess = 100.0f;
	float SpecularIntensity = Shininess;
	float4 specular = pow(saturate(max(0, dot(V, R))), Shininess) * SpecularColor * SpecularIntensity;
	if (dot(N,L) < 0) specular = 0;
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;

	color *= input.Color;
	return color;
}


//=========================================================================================================================================
//  LAB10 - Technique Block (With Texture)
//=========================================================================================================================================
technique ParticleWithTexture
{
	pass P0
	{
		VertexShader = compile vs_4_0 ParticleVertexShader();
		PixelShader = compile ps_4_0 ParticlePixelShader();
	}
}


//#########################################################################################################################################
//  ASSIGNMENT4 - Technique Block (Without Texture)
//#########################################################################################################################################
technique ParticleNoTexture
{
	pass P0
	{
		VertexShader = compile vs_4_0 ParticlePhongVertexShader();
		PixelShader = compile ps_4_0 ParticlePhongPixelShader();
	}
}