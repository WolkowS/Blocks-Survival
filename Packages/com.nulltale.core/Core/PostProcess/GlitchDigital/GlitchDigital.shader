Shader "Hidden/GlitchDigital"
{
    Properties
    {
        _Intensity ("Intensity", Float) = 1
    }
    
    CGINCLUDE

    #include "UnityCG.cginc"
    
    sampler2D _MainTex;
    sampler2D _GlitchTex;
    sampler2D _BufferTex;
    float _Intensity;
    
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

    frag_in vert(vert_in v)
    {
        frag_in o;
        o.vertex = v.vertex;
        o.uv = v.uv;
        return o;
    }
    
    float4 frag(v2f_img i) : SV_Target 
    {
        return tex2D(_MainTex, i.uv);
    }
    
    float4 frag_glitch(v2f_img i) : SV_Target 
    {
        float4 glitch = tex2D(_GlitchTex, i.uv);

        float thresh = 1.001 - _Intensity * 1.001;
        float w_d = step(thresh, pow(glitch.z, 2.5)); // Displacement glitch
        float w_b = step(thresh, pow(glitch.w, 2.5)); // Buffer glitch
        float w_c = step(thresh, pow(glitch.z, 3.5)); // Color glitch

        // Displacement.
        float2 uv = i.uv + glitch.xy * w_d;
        float4 source = tex2D(_MainTex, uv);

        // Mix with a buffer.
        float3 color = lerp(source, tex2D(_BufferTex, uv), w_b).rgb;

        // Shuffle color components.
        color = lerp(color, color - source.bbg * 2 + color.grr * 2, w_c);

        return float4(color, source.a);
    }

    ENDCG 
    
    Subshader
    {
        Pass    // 0
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }      
            CGPROGRAM
            //#pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag_glitch
            ENDCG
        }
        
        Pass    // 1
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }      
            CGPROGRAM
            //#pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
