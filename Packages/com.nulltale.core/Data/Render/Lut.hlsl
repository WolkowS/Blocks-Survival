#ifndef LUT16_INCLUDED
#define LUT16_INCLUDED


//#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
//#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

SamplerState _point_clamp_sampler;

real3 GetLinearToSRGB(real3 c)
{
#if _USE_FAST_SRGB_LINEAR_CONVERSION
    return FastLinearToSRGB(c);
#else
    return LinearToSRGB(c);
#endif
}

real3 GetSRGBToLinear(real3 c)
{
#if _USE_FAST_SRGB_LINEAR_CONVERSION
    return FastSRGBToLinear(c);
#else
    return SRGBToLinear(c);
#endif
}

/*real3 FastLinearToSRGB(real3 c)
{
    return saturate(1.055 * PositivePow(c, 0.416666667) - 0.055);
}
real3 LinearToSRGB(real3 c)
{
    real3 sRGBLo = c * 12.92;
    real3 sRGBHi = (PositivePow(c, real3(1.0/2.4, 1.0/2.4, 1.0/2.4)) * 1.055) - 0.055;
    real3 sRGB   = (c <= 0.0031308) ? sRGBLo : sRGBHi;
    return sRGB;
}
real3 FastSRGBToLinear(real3 c)
{
    return c * (c * (c * 0.305306011 + 0.682171111) + 0.012522878);
}
real3 SRGBToLinear(real3 c)
{
#if defined(UNITY_COLORSPACE_GAMMA) && REAL_IS_HALF
    c = min(c, 100.0); // Make sure not to exceed HALF_MAX after the pow() below
#endif
    real3 linearRGBLo  = c / 12.92;
    real3 linearRGBHi  = PositivePow((c + 0.055) / 1.055, real3(2.4, 2.4, 2.4));
    real3 linearRGB    = (c <= 0.04045) ? linearRGBLo : linearRGBHi;
    return linearRGB;
}*/

void lut_16_pix_float(in float3 col, in UnityTexture2D lut, out float3 result)
{
    // sample the texture
    float3 uvw = GetLinearToSRGB(col);

    float2 uv;
    uv.y = uvw.y * (15. / 16.) + 0.5 * (1. / 16.);
    uv.x = uvw.x * (15. / (16. * 16.)) + 0.5 * (1. / (16. * 16.)) + floor(uvw.z * 16.) / 16.;

    float3 lutColor = lut.Sample(_point_clamp_sampler, uv).rgb;

    lutColor = GetSRGBToLinear(lutColor);

    result = lutColor;

    //return real4(result.r, result.g, result.b, source.a);
}

#endif // LUT16_INCLUDED