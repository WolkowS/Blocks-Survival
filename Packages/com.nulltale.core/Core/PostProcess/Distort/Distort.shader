Shader "Hidden/Distort"
{
    Properties
    {
        _Settings("Settigns", Vector) = (0,0,0,0)
    }

    SubShader
    {
        name "Distort"
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
            
            sampler2D       _MainTex;
            float4          _Settings;  // x - sharpness, y - tiling, x - offset, w - weight
            
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
                // Sharpen calculations
                half4 initial = tex2D(_MainTex, i.uv);
                
                half4 sample  = tex2D(_MainTex, i.uv + float2(sin(i.uv.y * _Settings.y + _Settings.z) * _Settings.x, 0));

                // Output to screen
                return lerp(initial, sample, _Settings.w);
            }
            
            ENDHLSL
        }
    }
}