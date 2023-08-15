Shader "Hidden/ChromaticAberration"
{
    Properties
    {
        _Intensity ("Intensity", Range(0.0,1.0)) = 0.001
        _Center ("Center", Range(0.0,0.5)) = 0.0
        _Weight("Weight", Range(0, 1)) = 1
        _R ("R", Vector) = (1, 0, 0, 0)
        _G ("G", Vector) = (-1, 0, 0, 0)
        _B ("B", Vector) = (0, 1, 0, 0)
        [KeywordEnum(RECTANGLE, OFFSET)] _MODE ("Mode", Float) = 0
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
            
        Pass
        {
            Name "Chromatic Aberration"
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_local _MODE_RECTANGLE _MODE_OFFSET

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _MainTex;
            fixed _Intensity;
            fixed _Center;
            fixed _Weight;
            float2 _R;
            float2 _G;
            float2 _B;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            fixed4 sample(float mov, float2 uv)
            {
                float2 uvR = uv + _R * mov;
                float2 uvG = uv + _G * mov;
                float2 uvB = uv + _B * mov;
                
                fixed4 colR = tex2D(_MainTex, uvR);
                fixed4 colG = tex2D(_MainTex, uvG);
                fixed4 colB = tex2D(_MainTex, uvB);
                
                return fixed4(colR.r, colG.g, colB.b, 1);
            }

            fixed4 frag(v2f i) : COLOR
            {
                fixed4 result;
                
#ifdef _MODE_RECTANGLE
                float2 rectangle = float2(i.uv.x - _Center, i.uv.y - _Center);
                float dist = sqrt(pow(rectangle.x, 2) + pow(rectangle.y, 2));
                float mov = _Intensity * dist;
                
                result = sample(mov, i.uv);          
#endif

#ifdef _MODE_OFFSET
                float mov = _Intensity * .07;
                result = sample(mov, i.uv);
#endif
                return lerp(tex2D(_MainTex, i.uv), result, _Weight);
            }
            ENDCG
        }
    }
}