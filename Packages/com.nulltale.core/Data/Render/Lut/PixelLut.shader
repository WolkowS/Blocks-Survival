Shader "Unlit/PixelLut"
{
    Properties
    {
        [NoScaleOffset]
        _Lut ("Lut", 2D) = "white" {}
        _Weight("Weight", Range(0, 1)) = 1
        [Toggle(PIXEL)]_Pixel("Pixel", Float) = 1
    }
    SubShader
    {

        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 0

        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off

        Pass
        {

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Fullscreen.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile __ PIXEL
            //#pragma shader_feature PIXEL

            struct vert_in
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD0;
            };

            struct frag_in
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            SamplerState _point_clamp_sampler;
            SamplerState _linear_clamp_sampler;
            Texture2D    _MainTex;
            Texture2D    _Lut;
            float        _Weight;

            frag_in vert (vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            real4 frag (frag_in i) : SV_Target
            {
                // sample the texture
                real4 source = _MainTex.Sample(_point_clamp_sampler, i.uv);
                real3 uvw = GetLinearToSRGB(source.rgb);

                real2 uv;
                uv.y = uvw.y * (15. / 16.) + 0.5 * (1. / 16.);
                uv.x = uvw.x * (15. / (16. * 16.)) + 0.5 * (1. / (16. * 16.)) + floor(uvw.z * 16.) / 16.;

#ifdef PIXEL
                real3 lut = _Lut.Sample(_point_clamp_sampler, uv).rgb;
#else
                real3 lutA = _Lut.Sample(_linear_clamp_sampler, uv).rgb;
                uv.x += 1. / 16.;
                real remainder = uvw.z - floor(uvw.z * 16.) / 16.;
                real3 lutB = _Lut.Sample(_linear_clamp_sampler, uv).rgb;
                real3 lut = lerp(lutA, lutB, remainder).rgb;
#endif

                lut = GetSRGBToLinear(lut);

                // apply fog
                real3 result = lerp(source.rgb, lut, _Weight);

                return real4(result.r, result.g, result.b, source.a);
            }
            ENDHLSL
        }
    }
}
