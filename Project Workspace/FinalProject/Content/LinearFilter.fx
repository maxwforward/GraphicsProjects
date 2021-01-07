// Variables
float2 TextureSize;


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//  FROM ONLINE TUTORIAL - Pixel Shader Input Data Structure
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
struct PixelShaderInput
{
    float4 position : SV_POSITION;
    float4 color : COLOR0;
    float2 texCoord : TEXCOORD0;
};


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//  FROM ONLINE TUTORIAL - Sampler
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
sampler TextureSampler : register(s0);


//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
//  FROM ONLINE TUTORIAL - Pixel Shader
//>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
float4 PixelShaderFunction(PixelShaderInput input) : SV_TARGET0
{
    float xOffset = 1.0 / TextureSize.x;
    float yOffset = 1.0 / TextureSize.y;
    float2 TexCoords;
    float4 Color = tex2D(TextureSampler, input.texCoord);
    TexCoords.x = input.texCoord.x + xOffset;
    TexCoords.y = input.texCoord.y;
    Color += tex2D(TextureSampler, TexCoords);
    TexCoords.x = input.texCoord.x - xOffset;
    TexCoords.y = input.texCoord.y;
    Color += tex2D(TextureSampler, TexCoords);
    TexCoords.x = input.texCoord.x;
    TexCoords.y = input.texCoord.y + yOffset;
    Color += tex2D(TextureSampler, TexCoords);
    TexCoords.x = input.texCoord.x;
    TexCoords.y = input.texCoord.y - yOffset;
    Color += tex2D(TextureSampler, TexCoords);
    Color *= 0.2;
    return float4(Color.rgb, 1.0);
}


// Technique Block
technique LinearFilter
{
    pass P0
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}