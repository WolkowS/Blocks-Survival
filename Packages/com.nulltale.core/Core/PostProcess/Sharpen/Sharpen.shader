Shader "Hidden/Sharpen"
{
    Properties
    {
        _Impact("Impact", Range(0, 1)) = 1
        _Center("Center Weight", Float) = 5
        _Side("Sample Weight", Float) = -1
        
		[Toggle(BOX)]_Centered("Box", Float) = 1
    }

    SubShader
    {
        name "Sharpen"
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
            
			#pragma multi_compile_local __ BOX

            sampler2D       _MainTex;
            uniform float4  _MainTex_TexelSize;         // 1 / width, 1 / height, width, height
            
            half           _Impact;
            half           _Center;
            half           _Side;

            //half4x4        _kernel; 
            
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
                half4 result = initial * _Center;
                
                result += tex2D(_MainTex, i.uv + float2( _MainTex_TexelSize.x, 0)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, 0)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(0,-_MainTex_TexelSize.y)) * _Side;
#ifdef BOX
                result += tex2D(_MainTex, i.uv + float2( _MainTex_TexelSize.x, _MainTex_TexelSize.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2( _MainTex_TexelSize.x,-_MainTex_TexelSize.y)) * _Side;
                result += tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x,-_MainTex_TexelSize.y)) * _Side;
#endif

                // Output to screen
                return lerp(initial, result, _Impact);
            }
            ENDHLSL
        }
    }
}