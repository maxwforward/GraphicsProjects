//###################################################################################################################################################
//	SimplestShader.fx - Basic Shader Programming in HLSL (High Level Shader Programming)
//	Max Forward
//	Spring 2020
//	CPI 411 - Lab 1
//###################################################################################################################################################



//***************************************************************************************************************************************************
//	PART A.
//***************************************************************************************************************************************************
// This part contains the minimum HLSL code.  It requires three blocks: the vertex shader, the pixel shader, and the main function to set the vertex
// and pixel shaders.

/*
// Vertex Shader
float4 MyVertexShader(float4 position: POSITION) : POSITION
{
	return position;
}

// Pixel Shader
float4 MyPixelShader() : COLOR
{
	return float4(1, 1, 1, 1);
}

// Main Function
technique MyTechnique
{
	pass Pass1
	{

	}
}
*/


texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture>;
};

struct VertexPositionTexture
{
	float4 Position: POSITION;
	float2 TextureCoordinate: TEXCOORD;
};


// *** For VertexPositionColor
struct VertexPositionColor 
{ 
	float4 Position: POSITION;  
	float4 Color: COLOR; 
};

//##################################################################################################################################################
VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	return input;
}

float4 MyPixelShader(VertexPositionTexture input) : COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}
//##################################################################################################################################################


/*
VertexPositionColor MyVertexShader(VertexPositionColor input)
{ 
	return input; 
}

float4 MyPixelShader(VertexPositionColor input) : COLOR
{
	float4 color = input.Color;
	//color.r = 0;
	//color.rb = 0;
	//color.rb = color.br;
	//color = 1 - color;
	//color += 0.3f;
	if (color.r % 0.1 < 0.05f)
	{
		return float4(1, 1, 1, 1);
	} 
	else
	{
		return color;
	}
	return color;
}

float4 MyVertexShader(float4 position: POSITION) : POSITION
{
	return position;
}

float4 MyPixelShader() : COLOR
{
	return float4(1, 0, 0, 0.5f);
}
*/

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}