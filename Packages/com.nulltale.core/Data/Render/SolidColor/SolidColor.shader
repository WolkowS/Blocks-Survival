Shader "Unlit/SolidColor"
{
    Properties
    {
	    _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        ZTest Always
        ZWrite Off
        ZClip false
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vert_in
            {
                float4 vertex : POSITION;
            };

            struct frag_in
            {
                float4 vertex : SV_POSITION;
            };

            float4       _Color;

            frag_in vert (vert_in v)
            {
                frag_in o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (frag_in i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
