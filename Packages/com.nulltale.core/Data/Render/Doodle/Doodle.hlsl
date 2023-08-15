
void vert_float(in float4 pos, in float noiseScale, in float noiseSnap, out float4 doodle)
{
    float time = float3(snap(_Time.y, _noiseSnap), 0, 0);
    float3 noise = random3(pos.xyz + time) * _noiseScale;

    return pos + noise;
}

float3 random3(float3 c) 
{
    float j = 4096.0*sin(dot(c, float3(17.0, 59.4, 15.0)));
    float3 r;
    r.z = frac(512.0 * j);
    j *= 0.125;        
    r.x = frac(512.0 * j);
    j *= 0.125;        
    r.y = frac(512.0 * j);

    return r - 0.5;
}

inline float snap(float x, float snap)
{
    return snap * round(x / snap);
}