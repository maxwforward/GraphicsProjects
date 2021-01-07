// *** Lab02 ****
texture MyTexture;

// Part A
float3 offset;

// Part B
float4x4 World;
float4x4 View;
float4x4 Projection;


sampler mySampler = sampler_state 
{
	Texture = <MyTexture>;
};


struct VertexPositionTexture 
{
	float4 Position: POSITION;
	float2 TextureCoordinate : TEXCOORD;
};


// Vertex Shader
VertexPositionTexture MyVertexShader2(VertexPositionTexture input) 
{
	// Part A
	//input.Position.xyz += offset;
	//return input;

	// Part B
	VertexPositionTexture output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPos = mul(worldPos, View);
	output.Position = mul(viewPos, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	return output;
}


float4 MyPixelShader2(VertexPositionTexture input) : COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}


struct VertexPositionColor 
{
	float4 Position: POSITION;
	float4 Color: COLOR;
};


VertexPositionColor MyVertexShader(VertexPositionColor input) 
{
	return input;
}


float4 MyPixelShader(VertexPositionColor input) : COLOR
{
	//return float4(1, 0, 0, 0.5);
	float4 color = input.Color;
	if (color.r % 0.1 < 0.05f) return float4(1, 1, 1, 1);
	else return color;
	//return color;
}


technique MyTechnique 
{
	pass Pass1 
	{
		VertexShader = compile vs_4_0 MyVertexShader2();
		PixelShader = compile ps_4_0 MyPixelShader2();
	}
}