Shader "Hidden/Invert"
{
    Properties
    {
        _Weight("Weight", Range(0, 1)) = 1
    }

    SubShader
    {
        name "Invert"
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
            uniform float4  _MainTex_TexelSize;         // 1 / width, 1 / height, width, height
            half            _Weight; 
            
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
                half4 invert  = half4(half3(1, 1, 1) - initial.rgb, initial.a);

                // Output to screen
                return lerp(initial, invert, _Weight);
            }
            ENDHLSL
        }
    }
}