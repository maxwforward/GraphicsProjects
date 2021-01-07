float4x4 World; 
float4x4 View; 
float4x4 Projection; 
float4x4 WorldInverseTranspose; 
float3 CameraPosition; 
float3 LightPosition; 
float AmbientColor; 
float AmbientIntensity; 
float4 DiffuseColor; 
float DiffuseIntensity; 
float4 SpecularColor; 
float SpecularIntensity; 
float Shininess; 
texture normalMap;


sampler tsampler1 = sampler_state 
{
	texture = <normalMap>; 
	magfilter = LINEAR; // None, POINT, LINEAR, Anisotropic 
	minfilter = LINEAR; 
	mipfilter = LINEAR; 
	AddressU = Wrap; // Clamp, Mirror, MirrorOnce, Wrap, Border
	AddressV = Wrap;
}; 


struct VertexShaderInput
{
	float4 Position: POSITION0;
	float4 Normal: NORMAL0;
	float4 Tangent: TANGENT0;
	float4 Binormal: BINORMAL0;
	float2 TexCoord: TEXCOORD0;
};


struct VertexShaderOutput
{
	float4 Position: POSITION0;
	float3 Normal: TEXCOORD0;
	float3 Tangent: TEXCOORD1;
	float3 Binormal: TEXCOORD2;
	float2 TexCoord: TEXCOORD3;
	float3 WorldPosition: TEXCOORD4;
};


VertexShaderOutput BumpMapVS(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 projPosition = mul(viewPosition, Projection);
	output.Position = projPosition;
	output.WorldPosition = worldPosition.xyz;
	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);
	output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
	output.TexCoord = input.TexCoord; // ** UV

	return output;
}


float4 BumpMapPS(VertexShaderOutput input) : COLOR
{
	float3 L = normalize(LightPosition - input.WorldPosition);
	float3 V = normalize(CameraPosition - input.WorldPosition);
	float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3 B = normalize(input.Binormal);
	float3 H = normalize(L + V);

	float3 normalTex = (tex2D(tsampler1, input.TexCoord).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
	
	float3 bumpNormal = N + (normalTex.x * T + normalTex.y * B);
	// *** TBN Matrix Version
	//float3x3 TBN;
	//TBN[0] = T;
	//TBN[1] = B;
	//TBN[2] = N;
	//float3 bumpNormal = mul(normalTex, TBN);

	float3 diffuse = dot(bumpNormal, L);
	
	return float4(diffuse, 1);
	//return tex2D(tsampler1, input.TexCoord);
}


technique BumpMap
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 BumpMapVS();
		PixelShader = compile ps_4_0 BumpMapPS();
	}
}