Shader "Hidden/GradientMap"
{
    SubShader
    {
        name "Gradient Map"
        
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

            float  _Intensity;
            float4 _Weights;

            sampler2D    _MainTex;
            sampler2D    _Gradient;

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
                float4 initial  = tex2D(_MainTex, i.uv);
                float grayscale = dot(initial.rgb, _Weights.rgb);
                half4 result    = tex2D(_Gradient, float2(grayscale, 0));

                return half4(lerp(initial.rgb, result.rgb, _Intensity * result.a * _Weights.a), initial.a);
            }
            ENDHLSL
        }
    }
}