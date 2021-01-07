float4x4 World;
float4x4 View;
float4x4 Projection; 
float4x4 WorldInverseTranspose; 
float4 AmbientColor; 
float AmbientIntensity; 
float3 DiffuseLightDirection; 
float4 DiffuseColor; 
float DiffuseIntensity;

// We create structs to help us manage the inputs/outputs
// to vertex and pixel shaders
struct VertexInput
{
	// Here, POSITION0 and NORMAL0 are called mnemonics
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct VertexOutput
{
	float4 Position: POSITION;
	float4 Color: COLOR;
};

VertexOutput VS(VertexInput input)
{
	VertexOutput output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);

	float3 normal = mul(input.Normal, WorldInverseTranspose).xyz;
	float lightIntensity = max(0, dot(normal, normalize(DiffuseLightDirection)));
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);
	output.Color.w = 1;
	return output;
}

float4 PS(VertexOutput input) : COLOR
{
	return saturate(input.Color + AmbientColor * AmbientIntensity);
}

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VS();
		PixelShader = compile ps_4_0 PS();
	}
};