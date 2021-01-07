//------------------------------------------------------------------------------------------------------------
// From Lab03 - Global variables
//------------------------------------------------------------------------------------------------------------
float4x4 World; // World matrix
float4x4 View; // View matrix
float4x4 Projection; // Projection matrix
float4x4 WorldInverseTranspose; // Inverse Transpose
float4 AmbientColor;
float4 DiffuseColor;


//------------------------------------------------------------------------------------------------------------
// PART A - Add global variables
//------------------------------------------------------------------------------------------------------------
float3 CameraPosition; // in world space
float Shininess; // scalar value
float4 SpecularColor;
float SpecularIntensity;


float3 LightPosition; // in world space
float LightIntensity;


//------------------------------------------------------------------------------------------------------------
// From Lab03 - Input data structure
//------------------------------------------------------------------------------------------------------------
struct VertexShaderInput
{
	float4 Position: POSITION;
	float4 Normal: NORMAL;
	float2 UV: TEXCOORD0;
};



// --- Per Vertex technique - Gouraud
// We will create an output struct for this technique
struct GouraudVertexShaderOutput
{
	float4 Position: POSITION0;
	float4 Color: COLOR0;
};


// --- Per Pixel techniques - Phong, Blinn, Schlick
// We will need a different output struct for this
struct PhongVertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV: TEXCOORD0;
	float4 WorldPosition : TEXCOORD1;
	float4 Normal : TEXCOORD2;
};


// The Vertex Shader
// We will perform all the lighting calculations here
GouraudVertexShaderOutput GourandVertexShaderFunction(VertexShaderInput input)
{
	GouraudVertexShaderOutput output; // create the output struct

	// First do the transformations
	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// Now do the lighting computation
	// First calculate the directions
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz); // normalized surface normal vector
	float3 V = normalize(CameraPosition - worldPosition.xyz); // normalized vector toward the viewpoint
	float3 L = normalize(LightPosition); // normalized vector toward the light source
	float3 R = reflect(-L, N); // reflection vector

	// Now compute the light componenets
	float diffuse = max(0, dot(N, L));
	float4 specular = pow(max(0, dot(V, R)), Shininess);
	if (diffuse == 0) specular = 0;

	// Finally, save the color to the output struct
	output.Color = AmbientColor + (diffuse * DiffuseColor * LightIntensity) + ((specular * SpecularColor) * SpecularIntensity);
	output.Color.w = 1;
	return output;
}


// The Pixel Shader
// All this function does is to pass through the color through 
float4 GouraudPixelShaderFunction(GouraudVertexShaderOutput input) : COLOR0
{
	return input.Color;
}


// A common vertex shader for all the different techniques
PhongVertexShaderOutput PhongVertexShaderFunction(VertexShaderInput input)
{
	PhongVertexShaderOutput output;

	output.WorldPosition = mul(input.Position, World);
	float4 viewPosition = mul(output.WorldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// As well as the normal in world space
	output.Normal = mul(input.Normal, WorldInverseTranspose);
	output.UV = input.UV * 10;
	return output;
}


//*****************************************************************************************************************************************
//	PHONG
//*****************************************************************************************************************************************
// The pixel shader performs the lighting
float4 PhongPixelShaderFunction(PhongVertexShaderOutput input) : COLOR0
{
	// The lighting operation, same as in the Gouraud vertex method
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.Position.xyz);
	float3 L = normalize(LightPosition); // directional light
	float3 R = dot(-L, N);

	// Now compute the light components
	float diffuse = max(0, dot(N, L));
	float specular = pow(max(0, dot(V, R)), Shininess);
	if (diffuse == 0) specular = 0;
	return (AmbientColor + (diffuse * DiffuseColor * LightIntensity) + ((specular * SpecularColor) * SpecularIntensity));
}


//*****************************************************************************************************************************************
//	BLINN
//*****************************************************************************************************************************************
// The pixel shader performs the lighting
float4 BlinnPixelShaderFunction(PhongVertexShaderOutput input) : COLOR0
{
	// The lighting operation, same as in the Gouraud vertex method
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.Position.xyz);
	float3 L = normalize(LightPosition); // directional light
	float3 H = normalize(L + V);

	// Now compute the light components
	float diffuse = max(0, dot(N, L));
	float specular = pow(max(0, dot(H, N)), Shininess);
	if (diffuse == 0) specular = 0;
	return (AmbientColor + (diffuse * DiffuseColor * LightIntensity) + ((specular * SpecularColor) * SpecularIntensity));
}


//*****************************************************************************************************************************************
//	SCHLICK
//*****************************************************************************************************************************************
// The pixel shader performs the lighting
float4 SchlickPixelShaderFunction(PhongVertexShaderOutput input) : COLOR0
{
	// The lighting operation, same as in the Gouraud vertex method
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.Position.xyz); // vertex to eye
	float3 L = normalize(LightPosition); // directional light
	//float3 R = dot(-L, N);
	float3 R = normalize(2.0 * N * dot(N, L) - L);
	float3 t = dot(V, R);
	float schlick = (t / (Shininess - (Shininess * t) + t));

	// Now compute the light components
	//float diffuse = max(0, dot(N, L));
	float diffuse = DiffuseColor * max(0, dot(N, L));
	//float specular = t / (Shininess - t * Shininess + t);
	float specular = SpecularColor * schlick;
	//if (diffuse == 0) specular = 0;
	//return (AmbientColor + diffuse * DiffuseColor + specular * SpecularColor);
	return (AmbientColor + (diffuse * LightIntensity) + (specular * SpecularIntensity));
}


//*****************************************************************************************************************************************
//	TOON
//*****************************************************************************************************************************************
// The pixel shader performs the lighting
float4 ToonPixelShaderFunction(PhongVertexShaderOutput input) : COLOR0
{
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.Position.xyz);
	float3 L = normalize(LightPosition);
	float3 R = dot(-L, N);
	//float D = dot(V, R);
	float D = (dot(V, R) * SpecularIntensity);
	if (D < -0.7) 
	{
		//return float4(0, 0, 0, 1);
		return (float4(0, 0, 0, 1) * LightIntensity); 
	} 
	else if(D < 0.2) 
	{ 
		//return float4(0.25, 0.25, 0.25, 1);
		return (float4(0.25, 0.25, 0.25, 1) * LightIntensity);
	} 
	else if(D < 0.97) 
	{ 
		//return float4(0.5, 0.5, 0.5, 1);
		return (float4(0.5, 0.5, 0.5, 1) * LightIntensity);
	}
	else 
	{ 
		//return float4(1, 1, 1, 1);
		return (float4(1, 1, 1, 1) * LightIntensity);
	}
}


//*****************************************************************************************************************************************
//	HalfLife
//*****************************************************************************************************************************************
// The pixel shader performs the lighting
float4 HalfLifePixelShaderFunction(PhongVertexShaderOutput input) : COLOR0
{
	// The lighting operation, same as in the Gouraud vertex method
	float3 N = normalize(input.Normal.xyz);
	float3 V = normalize(CameraPosition - input.Position.xyz);
	float3 L = normalize(LightPosition); // directional light
	float3 R = dot(-L, N);

	// Now compute the light components
	float diffuse = (0.5 * (N * L + 1));
	float specular = pow(max(0, dot(V, R)), Shininess);
	if (diffuse == 0) specular = 0;
	return (AmbientColor + (diffuse * DiffuseColor * LightIntensity) + ((specular * SpecularColor) * SpecularIntensity));
}


// Now the different techniques
technique Gourand
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 GourandVertexShaderFunction();
		PixelShader = compile ps_4_0 GouraudPixelShaderFunction();
	}
}

technique Phong
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
}

technique Blinn
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 BlinnPixelShaderFunction();
	}
}

technique Schlick
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 SchlickPixelShaderFunction();
	}
}

technique Toon
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 ToonPixelShaderFunction();
	}
}

technique HalfLife
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 HalfLifePixelShaderFunction();
	}
}