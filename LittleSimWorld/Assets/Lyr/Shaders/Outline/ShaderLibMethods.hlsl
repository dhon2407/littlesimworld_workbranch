void CheckIfEdgeCoord_float(Texture2D txt, float Width, float2 uv, float2 Texel, SamplerState Sampler, out float IsEdge, out float edgeDistance)
{

	float x = 1 / Texel.x;
	float y = 1 / Texel.y;

	bool hasHitEdge = false;
	edgeDistance = 0;
	
	for (float i = 1; i <= Width; i++)
	{
		float l = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, 0) * i).a;
		float r = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, 0) * i).a;
		float u = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(0, y) * i).a;
		float d = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(0, -y) * i).a;

		float dTR = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, y) * i).a;
		float dTL = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, y) * i).a;
		float dBR = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(x, -y) * i).a;
		float dBL = SAMPLE_TEXTURE2D(txt, Sampler, uv + float2(-x, -y) * i).a;
		
		float edge = l + r + u + d + dTR + dTL + dBR + dBL;

		if (edge >= 0.1 && !hasHitEdge)
		{
			hasHitEdge = true;
			edgeDistance = i;
		}
	}
	
	IsEdge = (hasHitEdge ? 1 : 0);	
}