Shader "Unlit/Outline"
{
	Properties 
	{
		_Thickness("Thickness", Range(0.0005, 0.0050)) = 0.001
		_Sensitive("Sensitive", Float) = 50
		_OutlineColor("OutlineColor", Color) = (0, 0, 0 ,1)
	}
	SubShader 
	{
		name "Outline"
		
		Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline" }
		LOD 200
		
        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off
		
		Pass
		{
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            
			#pragma vertex vert
			#pragma fragment frag
            
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float  _Thickness;
            float  _Sensitive;
			float4 _OutlineColor;
            
            struct vert_in
            {
                float4 pos : POSITION;
                float2 uv  : TEXCOORD0;
            };

            struct frag_in
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float SampleDepth(float2 uv)
            {
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                return SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
                return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
#endif
            }
            float sobel(float2 uv) 
            {
                float2 delta = float2(_Thickness, _Thickness);
                
                float hr = 0;
                float vt = 0;
                
                hr += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                hr += SampleDepth(uv + float2( 1.0, -1.0) * delta) * -1.0;
                hr += SampleDepth(uv + float2(-1.0,  0.0) * delta) *  2.0;
                hr += SampleDepth(uv + float2( 1.0,  0.0) * delta) * -2.0;
                hr += SampleDepth(uv + float2(-1.0,  1.0) * delta) *  1.0;
                hr += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;
                
                vt += SampleDepth(uv + float2(-1.0, -1.0) * delta) *  1.0;
                vt += SampleDepth(uv + float2( 0.0, -1.0) * delta) *  2.0;
                vt += SampleDepth(uv + float2( 1.0, -1.0) * delta) *  1.0;
                vt += SampleDepth(uv + float2(-1.0,  1.0) * delta) * -1.0;
                vt += SampleDepth(uv + float2( 0.0,  1.0) * delta) * -2.0;
                vt += SampleDepth(uv + float2( 1.0,  1.0) * delta) * -1.0;
                
                return sqrt(hr * hr + vt * vt);
            }
            
            frag_in vert(vert_in input)
            {
                frag_in output;
                output.vertex = input.pos;
                output.uv     = input.uv;
                
                return output;
            }
            
            half4 frag (frag_in input) : SV_Target 
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float s   = pow(1 - saturate(sobel(input.uv)), _Sensitive);
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                return lerp(col, _OutlineColor, 1.0f - s);
            }
			
			ENDHLSL
		}
	}
}
