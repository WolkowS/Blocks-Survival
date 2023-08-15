Shader "Hidden/ArtisticFeature"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 0
        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off

        Pass     // 0
        {
            Name "Flip"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D    _MainTex;
            
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

            // =======================================================================
            frag_in vert(vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (frag_in i) : SV_Target
            {
                return tex2D(_MainTex, float2(1. - i.uv.x, i.uv.y));
            }
            ENDHLSL
        }
        
        Pass     // 1
        {
            Name "Grayscale"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D _MainTex;
            
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

            // =======================================================================
            frag_in vert(vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }
            
            half4 frag(frag_in i) : SV_Target
            {                
                half4 color = tex2D(_MainTex, i.uv);
                half val = dot(color, half4(0.299, 0.587, 0.114, 0));
                
                return half4(val, val, val, color.a); 
            }
            ENDHLSL
        }
        
        Pass     // 2
        {
            Name "Guidelines"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            
            struct vert_in
            {
                float4 vertex : POSITION;
            };

            struct frag_in
            {
                float4 vertex : SV_POSITION;
            };

            frag_in vert(vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                return o;
            }
            
            half4 frag(frag_in i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }

        Pass     // 3
        {
            name "Blit"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            sampler2D    _MainTex;
            
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

            // =======================================================================
            frag_in vert(vert_in v)
            {
                frag_in o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            half4 frag(frag_in i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                return color;
            }
            ENDHLSL
        }
    }
}