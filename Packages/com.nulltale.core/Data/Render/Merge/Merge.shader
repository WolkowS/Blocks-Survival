Shader "Hidden/Merge"
{
    Properties
    {
        [NoScaleOffset][MainTexture] _Overlay("Base (RGB)", 2D) = "white" {}
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

            float  _Sensitivity;

            Texture2D    _MainTex;
            Texture2D    _Overlay;
            SamplerState _point_clamp_sampler;

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
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (frag_in i) : SV_Target
            {
                // Pixel colour
                half4 source = _MainTex.Sample(_point_clamp_sampler, i.uv);
                half4 overlay = _Overlay.Sample(_point_clamp_sampler, i.uv);

                return lerp(source, overlay, overlay.a);
            }
            ENDHLSL
        }
    }
}