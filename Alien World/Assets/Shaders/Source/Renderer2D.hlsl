struct VSInput
{
	float4 position : POSITION;
	float2 uv : TEXCOORD;
	float tid : ID;
	float4 color : COLOR;
};

struct VSOutput
{
	float4 positionCS : SV_POSITION;
	float4 position : POSITION;
	float2 uv : TEXCOORD;
	float tid : ID;
	float4 color : COLOR;
};

cbuffer VSUniforms : register(b0)
{
	float4x4 sys_ProjectionMatrix;
	float4x4 sys_ViewMatrix;
}

VSOutput VSMain(VSInput input)
{
	VSOutput output;
	output.positionCS = mul(input.position, sys_ProjectionMatrix);
	output.position = input.position;
	output.uv = input.uv;
	output.tid = input.tid;
	output.color = input.color;
	return output;
}

Texture2D textures[32] : register(t0);
SamplerState samplers[32] : register(s0);

float4 PSMain(VSOutput input) : SV_TARGET
{
	float4 texColor = input.color;
	switch (input.tid)
	{
	case 1:
		texColor *= textures[0].Sample(samplers[0], input.uv);
		break;
	case 2:
		texColor *= textures[1].Sample(samplers[1], input.uv);
		break;
	case 3:
		texColor *= textures[2].Sample(samplers[2], input.uv);
		break;
	case 4:
		texColor *= textures[3].Sample(samplers[3], input.uv);
		break;
	case 5:
		texColor *= textures[4].Sample(samplers[4], input.uv);
		break;
	case 6:
		texColor *= textures[5].Sample(samplers[5], input.uv);
		break;
	}
	return texColor;
};
