Shader "Hidden/ShadowGrain"
{
    Properties
    {
        _Intensity("Intensity", Float) = 1
        _Response("Response", Float) = 256
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _Tiling("Tiling", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 0

        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            float  _Intensity;
            float  _Response;
            float4 _Tiling;
            float4 _Tint;

            Texture2D    _MainTex;
            Texture2D    _GrainTex;
            SamplerState _point_repeat_sampler;

            struct vert_in
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct frag_in
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            frag_in vert (vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            half4 frag (frag_in i) : SV_Target
            {
                // Pixel colour
                half4 source = _MainTex.Sample(_point_repeat_sampler, i.uv);

                half grain = _GrainTex.Sample(_point_repeat_sampler, i.uv * _Tiling.xy + _Tiling.zw).w;
                grain = (grain - 0.5) * _Intensity;
                //grain = grain + abs(grain);

                half lum = pow(1 - dot(source.rgb, half3(0.3, 0.59, 0.11)), _Response);

                return source + grain * lum * _Tint;
            }
            ENDHLSL
        }
    }
}