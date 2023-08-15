Shader "Sprites/Doodle"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_NoiseScale("Noise impact", Float) = 0.01
		_NoiseSnap("Noise interval", Float) = 0.3
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
            "DisableBatching" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ PIXELSNAP_ON
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                };

                struct frag_in
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord  : TEXCOORD0;
                };

                fixed4 _Color;
                float _NoiseScale;
                float _NoiseSnap;

                float3 random3(float3 c) 
                {
                    float j = 4096.0*sin(dot(c, float3(17.0, 59.4, 15.0)));
                    float3 r;
                    r.z = frac(512.0 * j);
                    j *= 0.125;        
                    r.x = frac(512.0 * j);
                    j *= 0.125;        
                    r.y = frac(512.0 * j);

                    return r - 0.5;
                }

                inline float snap(float x, float snap)
                {
                    return snap * round(x / snap);
                }

                frag_in vert(appdata_t IN)
                {
                    frag_in OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color * _Color;

                    float time = float3(snap(_Time.y, _NoiseSnap), 0, 0);
                    float2 noise = random3(IN.vertex.xyz + time).xy * _NoiseScale;
                    OUT.vertex.xy += noise;

#ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

                    return OUT;
                }

                sampler2D _MainTex;
                sampler2D _AlphaTex;
                float _AlphaSplitEnabled;

                fixed4 SampleSpriteTexture(float2 uv)
                {
                    fixed4 color = tex2D(_MainTex, uv);

    #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                    if (_AlphaSplitEnabled)
                        color.a = tex2D(_AlphaTex, uv).r;
    #endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

                    return color;
                }

                fixed4 frag(frag_in IN) : SV_Target
                {
                    fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                    c.rgb *= c.a;
                    return c;
                    //return tex2D(_MainTex, IN.texcoord);
                }
            ENDCG
		}
	}
}