Shader "Hidden/Blur"
{
    Properties
    {
        _Directions("Directions", Range(1, 32)) = 16
        _Steps("_Steps", Range(1, 7)) = 3
        _Radius("Radius", Range(0.001, 0.12)) = 0.07
        _Rotation("Rotation", Float) = 0.00
        _Radial("Radial", Float) = 0.00
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
            Name "Blur"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 

            float _Directions;
            float _Steps;
            float _Radius;
            float _Rotation;
            float _Radial;

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
                half4 result = tex2D(_MainTex, i.uv);
                float step   = 1 / _Steps;
                float radius = (distance(i.uv, float2(.5, .5)) * _Radial - 1) * _Radius;

                // Blur calculations
                for (float d = 0; d < TWO_PI; d += TWO_PI / _Directions)
                    for (float n = step; n <= 1; n += step)
                        result += tex2D(_MainTex, i.uv + float2(cos(d + _Rotation), sin(d + _Rotation)) * radius * n * n);

                // Output to screen
                result /= floor(_Steps) * ceil(_Directions) + 1;
                return result;
            }
            ENDHLSL
        }
    }
}