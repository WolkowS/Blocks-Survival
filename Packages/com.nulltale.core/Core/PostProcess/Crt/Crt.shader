Shader "Hidden/Crt"
{
    Properties
    {
        _Distortion("_Distortion", Vector) = (1, 1, 0, 0)
        _Scanlines("_Scanlines", Vector) = (400, 1.3, .2, .03)
    }

    SubShader
    {
        name "Crt"
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
            
            Texture2D    _MainTex;
            SamplerState _point_repeat_sampler;
            sampler2D    _NoiseTex;
            
            float4       _NoiseOffset;
            float4       _Distortion;
            float4       _Scanlines;
            
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
        
            float3 scanlines(in float3 col, in float2 uv)
            {
                float2 sl = float2(sin(uv.y * _Scanlines.x), cos(uv.y * _Scanlines.x));
	            float3 scanlines = float3(sl.x, sl.y, sl.x);

                col += col * scanlines * _Scanlines.y;
                col += col * _Scanlines.z;
                
                return col;
            }
            
            half4 frag (frag_in i) : SV_Target
            {
                float offset = frac(sin(i.uv.y  + _Time.x * _Distortion.y) * _Distortion.z) * _Distortion.x;
                float2 uv = i.uv;
                uv.x += offset;
                
                float4 main = _MainTex.Sample(_point_repeat_sampler, uv + float2(0, _Distortion.w));
                float4 noise = tex2D(_NoiseTex, uv + _NoiseOffset.xy);
                float  alpha = tex2D(_NoiseTex, uv + _NoiseOffset.zw).a;
                float  s = step(alpha, _Scanlines.w);
                
                // Output to screen
                return half4(scanlines(lerp(main, noise, s).xyz, i.uv), main.a);
            }
            ENDHLSL
        }
    }
}