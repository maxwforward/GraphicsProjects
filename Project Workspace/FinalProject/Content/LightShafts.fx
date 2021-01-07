// Variables
float3 LightPosition;
float Density;
float Decay;
float Weight;
float Exposure;


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
    int NumberOfSamples = 100;
    float2 TexCoord = input.texCoord;
    float2 PixelToLightVector = (TexCoord.xy - LightPosition.xy);
    float Length = length(PixelToLightVector);
    PixelToLightVector *= 1.0 / NumberOfSamples * Density;
    float4 Color = tex2D(TextureSampler, TexCoord);
    float IlluminationDecay = 1.0;
    for (int i = 0; i < NumberOfSamples; ++i)
    {
        TexCoord -= PixelToLightVector;
        float4 Sample = tex2D(TextureSampler, TexCoord);
        Sample *= IlluminationDecay * Weight;
        Color += Sample;
        IlluminationDecay *= Decay;
    }
    return float4(Color.xyz * Exposure, 1.0);
}


// Technique Block
technique LightShafts
{
    pass P0
    {
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}