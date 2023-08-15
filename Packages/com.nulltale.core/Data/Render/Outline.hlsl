
void outerPix_float(in UnityTexture2D tex, in float2 uv, out float outline)
{
	float source = tex2D(tex, uv).a;
	float value = 
		tex2D(tex, uv + float2(0.0f, tex.texelSize.y)).a + 
		tex2D(tex, uv + float2(0.0f, -tex.texelSize.y)).a + 
		tex2D(tex, uv + float2(tex.texelSize.x, 0.0f)).a + 
		tex2D(tex, uv + float2(-tex.texelSize.x, 0.0f)).a;
	
	outline = max(min(value, 1) - source, 0);
}

void innerPix_float(in UnityTexture2D tex, in float2 uv, out float outline)
{
	float value = 
		tex2D(tex, uv + float2(0.0f, tex.texelSize.y)).a * 
		tex2D(tex, uv + float2(0.0f, -tex.texelSize.y)).a * 
		tex2D(tex, uv + float2(tex.texelSize.x, 0.0f)).a * 
		tex2D(tex, uv + float2(-tex.texelSize.x, 0.0f)).a;
	
	outline = tex2D(tex, uv).a  - value;
}


void outerPixSteps_float(in UnityTexture2D tex, in float2 uv, in int steps, out float outline)
{
	float source = tex2D(tex, uv).a;
    float value = 0.0f;
    
    for(int i = 1; i <= steps; i++)
    {
        value += tex2D(tex, uv + float2(0.0f, tex.texelSize.y * i)).a;
        value += tex2D(tex, uv + float2(0.0f, -tex.texelSize.y * i)).a;
        value += tex2D(tex, uv + float2(tex.texelSize.x * i, 0.0f)).a; 
        value += tex2D(tex, uv + float2(-tex.texelSize.x * i, 0.0f)).a;
    }
	
	outline = max(min(value, 1) - source, 0);
}

void outer_float(in UnityTexture2D tex, in float2 uv, in float offset, out float outline)
{
	float source = tex2D(tex, uv).a;
    float value = 0.0f;
    
    value += tex2D(tex, uv + float2(0.0f, tex.texelSize.y * offset)).a;
    value += tex2D(tex, uv + float2(0.0f, -tex.texelSize.y * offset)).a;
    value += tex2D(tex, uv + float2(tex.texelSize.x * offset, 0.0f)).a; 
    value += tex2D(tex, uv + float2(-tex.texelSize.x * offset, 0.0f)).a;
    
    value += tex2D(tex, uv + float2(tex.texelSize.x * offset, tex.texelSize.y * offset)).a; 
    value += tex2D(tex, uv + float2(tex.texelSize.x * offset, -tex.texelSize.y * offset)).a;
    value += tex2D(tex, uv + float2(-tex.texelSize.x * offset, tex.texelSize.y * offset)).a; 
    value += tex2D(tex, uv + float2(-tex.texelSize.x * offset, -tex.texelSize.y * offset)).a;
	
	outline = min(value, 1) - source;
}

void gaussian_blur_float(in UnityTexture2D tex, in float2 uv, out float4 color)
{
    float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = 16.0;    // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 3.0;        // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = 8.0;           // BLUR SIZE (Radius)
    // GAUSSIAN BLUR SETTINGS }}}
   
    float2 Radius = Size * tex.texelSize.xy;
    
    // Normalized pixel coordinates (from 0 to 1)
    // Pixel colour
    float4 result = tex2D(tex, uv);
    
    // Blur calculations
    for( float d = 0.0; d<Pi; d+=Pi/Directions)
    {
		for(float i = 1.0 / Quality; i <= 1.0; i += 1.0 / Quality)
        {
			result += tex2D(tex, uv + float2(cos(d),sin(d))*Radius*i);		
        }
    }
    
    // Output to screen
    result /= Quality * Directions - 15.0;
    color =  result;
}