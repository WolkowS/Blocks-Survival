Shader "Hidden/VHSPostProcess" {
	Properties 
	{
		_Intensity ("Intensity", Range(0.0, 1.0)) = 1
		_Rocking ("Rocking", Range(0.0, 0.1)) = 0.01
		_VHSTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _VHSTex;
			
			float _yScanline;
			float _xScanline;
			float _Intensity;
			float _Rocking;
			
			float rand(float3 co)
			{
			     return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
			}
 
			fixed4 frag (v2f_img i) : COLOR
			{
				const fixed4 main = tex2D(_MainTex, i.uv);
				fixed4 vhs = tex2D (_VHSTex, i.uv);
				
				float dx = 1 - abs(distance(i.uv.y, _xScanline));
				float dy = 1 - abs(distance(i.uv.y, _yScanline));
				
				i.uv.x += (dy - .5) * _Rocking + rand(float3(dy, dy, dy)).r / 500;
								
				if (dx > 0.99)
					i.uv.y = _xScanline;
				
				i.uv = frac(i.uv);
				
				const fixed4 c = tex2D(_MainTex, i.uv);
				
				float bleed = tex2D(_MainTex, i.uv + float2(0.01, 0)).r;
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0)).r;
				bleed += tex2D(_MainTex, i.uv + float2(0.01, 0.01)).r;
				bleed += tex2D(_MainTex, i.uv + float2(0.02, 0.02)).r;
				bleed /= 6;
				
				if (bleed > 0.1)
				{
					vhs += fixed4(bleed * _xScanline, 0, 0, 0);
				}
								
				vhs += c - rand(float3(i.uv.x, i.uv.y, _xScanline)) * _xScanline / 5;
				return lerp(main, vhs, _Intensity);
			}
			ENDCG
		}
	}
Fallback off
}