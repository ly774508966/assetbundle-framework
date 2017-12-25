Shader "Game4f/Mix Masking(Without Moving)" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _TintColor ("TintColor", Color) = (1,1,1,1)
        _Exposure ("Exposure", Range(1,10)) = 1
        _SpeedX ("SpeedX", Float) = 10.0
        _SpeedY ("SpeedY", Float) = 0
    }

	CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _NoiseTex;

		half4 _MainTex_ST;
		half4 _NoiseTex_ST;
		float _Exposure;

		fixed4 _TintColor;
		float _SpeedX;
		float _SpeedY;

		struct v2f {
			half4 pos : SV_POSITION;
			half4 uv : TEXCOORD0;
		};

		v2f vert(appdata_full v)
		{
			v2f o;

			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv.zw = TRANSFORM_TEX(v.texcoord, _NoiseTex);

			return o;
		}

		fixed4 frag( v2f i ) : COLOR
		{
			float4 sp = _Time;
			float2 delta;
			delta.x = sp.x * _SpeedX;
			delta.y = sp.x * _SpeedY; // = float2(sp.x * 10.0, sp.x * 0.0); ////----
			return tex2D (_MainTex, i.uv.xy + delta) * tex2D (_NoiseTex, i.uv.zw) * _TintColor * _Exposure;
		}

	ENDCG

	SubShader {
		Tags { "RenderType" = "Transparent" "Reflection" = "LaserScope" "Queue" = "Transparent"}
		Cull Off
		ZWrite Off
		Blend SrcAlpha One

	Pass {

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		ENDCG

		}

	}
	FallBack Off
}
