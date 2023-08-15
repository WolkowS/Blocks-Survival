Shader "Hidden/Pixelation"
{
    Properties
    {
		_Pixels("Pixels", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" }
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

            sampler2D _MainTex;

            float2 _Pixels;
            float4 _Color;

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


            inline float2 uvSnap(float2 uv)
            {
                return float2(round((uv.x - 0.5) * _Pixels.x) / _Pixels.x + 0.5, round((uv.y - 0.5) * _Pixels.y) / _Pixels.y + 0.5);
            }

            frag_in vert(const vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            half4 frag(const frag_in i) : SV_Target
            {
                half4 col = tex2D(_MainTex, uvSnap(i.uv));
                return col;
            }
            ENDHLSL
        }
    }
}
