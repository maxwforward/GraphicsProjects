//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
float3 LightPosition;
texture NormalMap;
// Light Uniforms
float AmbientColor;
float AmbientIntensity;
float4 DiffuseColor;
float DiffuseIntensity;
float4 SpecularColor;
float SpecularIntensity;
float Shininess;


//=========================================================================================================================================
//  ASSIGNMENT3
//=========================================================================================================================================
// Bump Mapping Uniforms
float BumpHeight;
float NormalMapRepeatU;
float NormalMapRepeatV;
int MipMap;


//-----------------------------------------------------------------------------------------------------------------------------------------
//  FROM ASSIGNMENT2
//-----------------------------------------------------------------------------------------------------------------------------------------
texture EnvironmentMap;


//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
sampler NormalMapSamplerLinear = sampler_state
{
	texture = <NormalMap>;
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
};


//=========================================================================================================================================
//  ASSIGNMENT3
//=========================================================================================================================================
sampler NormalMapSamplerNone = sampler_state 
{ 
	texture = <NormalMap>; 
	magfilter = None;
	minfilter = None;        
	mipfilter = None;    
	AddressU = Wrap;    
	AddressV = Wrap; 
};


//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <EnvironmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};


//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float4 Tangent : TANGENT0;
	float4 Binormal : BINORMAL0;
	float2 TexCoord : TEXCOORD0;
};


//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float3 Tangent : TEXCOORD1;
	float3 Binormal : TEXCOORD2;
	float2 TexCoord : TEXCOORD3;
	float3 Position3D : TEXCOORD4;
};


//#########################################################################################################################################
//  VERTEX SHADER FOR ALL
//#########################################################################################################################################
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	float2 UVScale = float2(NormalMapRepeatU, NormalMapRepeatV);

	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord * UVScale;
	return output;
}


//#########################################################################################################################################
//  TANGETNT SPACE NORMAL PS
//#########################################################################################################################################
float4 TangentSpaceNormPS(VertexShaderOutput input) : COLOR
{
	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	if (MipMap == 0) return tex2D(NormalMapSamplerNone, input.TexCoord);
	else return tex2D(NormalMapSamplerLinear, input.TexCoord);
}


//#########################################################################################################################################
//  WORLD SPACE NORMAL PS
//#########################################################################################################################################
float4 WorldSpaceNormPS(VertexShaderOutput input) : COLOR
{
	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	if (MipMap == 0) return tex2D(NormalMapSamplerNone, input.TexCoord);
	else return tex2D(NormalMapSamplerLinear, input.TexCoord);
}


//#########################################################################################################################################
//  BUMP MAP PS
//#########################################################################################################################################
float4 BumpMapPS(VertexShaderOutput input) : COLOR
{
	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);

	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	float3 normalTex = (tex2D(NormalMapSamplerLinear, input.TexCoord).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
	if (MipMap == 0) normalTex = (tex2D(NormalMapSamplerNone, input.TexCoord).xyz - float3(0.5, 0.5, 0.5)) * 2.0;

	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	// float3 bumpNormal = N + (normalTex.x * T + normalTex.y * B);
	float3x3 TBN;
	TBN[0] = T;
	TBN[1] = B;
	TBN[2] = N;
	float3 bumpNormal = mul(normalTex, TBN); // TBN Matrix Version
	float3 diffuse = dot(bumpNormal, L);
	return float4(diffuse, 1);
}


//#########################################################################################################################################
//  BUMP REFLECT PS
//#########################################################################################################################################
float4 BumpReflectPS(VertexShaderOutput input) : COLOR
{
	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);
	
	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	float3 I = normalize(input.Position3D - CameraPosition);
	float3 normalTex = tex2D(NormalMapSamplerLinear, input.TexCoord).xyz;
	if (MipMap == 0)
	{
		normalTex = tex2D(NormalMapSamplerNone, input.TexCoord).xyz;
	}
	normalTex = 2.0 * (normalTex - 0.5);
	normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
	normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
	normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
	float3 bumpNormal = mul(normalTex, float3x3(T, B, N));
	float3 R = reflect(I, bumpNormal);
	float4 reflectedColor = texCUBE(SkyBoxSampler, R);
	return reflectedColor;
}


//#########################################################################################################################################
//  BUMP REFRACT PS
//#########################################################################################################################################
float4 BumpRefractPS(VertexShaderOutput input) : COLOR
{
	//*************************************************************************************************************************************
	//  FROM LAB07
	//*************************************************************************************************************************************
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);
	
	//=====================================================================================================================================
	//  ASSIGNMENT3
	//=====================================================================================================================================
	float3 I = normalize(input.Position3D - CameraPosition);
	float3 normalTex = tex2D(NormalMapSamplerLinear, input.TexCoord).xyz;
	if (MipMap == 0)
	{
		normalTex = tex2D(NormalMapSamplerNone, input.TexCoord).xyz;
	}
	normalTex = 2.0 * (normalTex - 0.5);
	normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
	normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
	normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
	float3 bumpNormal = mul(normalTex, float3x3(T, B, N));
	float3 R = refract(I, bumpNormal, 0.7);
	float4 refractedColor = texCUBE(SkyBoxSampler, R);
	return refractedColor;
}










//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
VertexShaderOutput VertexShaderFunction2(VertexShaderInput input)
{
	VertexShaderOutput output; 
	float4 worldPosition = mul(input.Position, World); 
	float4 viewPosition = mul(worldPosition, View); 
	output.Position = mul(viewPosition, Projection);
	
	/* 
	// to transform from object space to tangent space
	float3x3 objectToTangentSpace;
	objectToTangentSpace[0] = input.Tangent;
	objectToTangentSpace[1] = input.Binormal;
	objectToTangentSpace[2] = input.Normal;
	// [ Tx, Ty, Tz ] [ Objx ] = [ Tanx ]
	// [ Bx, By, Bz ] [ Objy ] = [ Tany ]
	// [ Nx, Ny, Nz ] [ Objz ] = [ Tanz ]
	output.Normal = mul(objectToTangentSpace, input.Normal); // object -> tangent?
	output.Tangent = mul(objectToTangentSpace, input.Tangent);
	output.Binormal = mul(objectToTangentSpace, input.Binormal);
	//
	*/

	// World Space looks better IMO
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	output.Position3D = worldPosition.xyz;
	output.TexCoord = input.TexCoord;// *UVScale;
	
	return output;
}


//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
float4 PixelShaderFunction2(VertexShaderOutput input) : COLOR0
{

	// Vectors: L = Light; V = View; N = Normal; T = Tangent; B = Binormal; H = Halfway between L and V
	float3 L = normalize(LightPosition - input.Position3D);
	float3 V = normalize(CameraPosition - input.Position3D);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);
	
	// Calculate the normal, including the information in the bump map
	float3 normalTex = tex2D(NormalMapSamplerLinear, input.TexCoord).xyz;
	normalTex = 1.0 * 2.0 * (normalTex - float3(0.5, 0.5, 0.5)); 
	//expand(normalTex);
	
	// *** Lab7 ********
	//float3 bumpNormal = normalize(N + (normalTex.x * T + normalTex.y * B));
	// (Lab7 Option MonoGame3.4) 
	// If does not work, use the OPTION-A
	//float3 bumpNormal = normalize(N + (normalTex.x * float3(1, 0, 0) + normalTex.y * float3(0, 1, 0))); // OPTION A
	
	// *** for Assignment3 ***
	float3x3 TangentToWorld;
	TangentToWorld[0] = (input.Tangent); 
	TangentToWorld[1] = (input.Binormal); 
	TangentToWorld[2] = (input.Normal); 
	float3 bumpNormal = mul(normalTex, TangentToWorld);
	
	//calculate Diffuse Term:
	float4 diffuse = DiffuseColor * DiffuseIntensity * max(0, (dot(bumpNormal, L)));
	diffuse.a = 1.0;
	
	// calculate Specular Term (H,N):
	float4 specular = SpecularColor * SpecularIntensity * pow(saturate(dot(H, bumpNormal)), Shininess);
	specular.a = 1.0;
	
	// Compute Final Color
	float4 finalColor = diffuse + specular; //ambient + diffuse + specular;
	return finalColor;
}










//*****************************************************************************************************************************************
//  FROM LAB07
//*****************************************************************************************************************************************
technique TangentSpaceNorm
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 TangentSpaceNormPS();
	}
}
technique WorldSpaceNorm
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 WorldSpaceNormPS();
	}
}
technique BumpMap
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 BumpMapPS();
	}
}
technique BumpReflect
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 BumpReflectPS();
	}
}
technique BumpRefract
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 BumpRefractPS();
	}
}