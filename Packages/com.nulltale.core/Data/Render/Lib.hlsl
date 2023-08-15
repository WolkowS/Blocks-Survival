

void chromatic_float(in UnityTexture2D tex, in float2 uv, in float2 vec, out float4 color)
{
	float4 red   = tex2D(tex, uv - tex.texelSize.xy * vec) * float4(1, 0, 0, 1);
	float4 green = tex2D(tex, uv) * float4(0, 1, 0, 1);
	float4 blue  = tex2D(tex, uv + tex.texelSize.xy * vec) * float4(0, 0, 1, 1);
		
	red.rgb   *= red.a;
	green.rgb *= green.a;
	blue.rgb  *= blue.a;
	
	color   = red + green + blue;
	color.a = red.a / 3. + green.a / 3. + blue.a / 3.;
}