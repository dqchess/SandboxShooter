﻿Shader "Eurus/DisbalanceBar"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_FillColor("Fill Color", Color) = (1,1,1,1)
		_BGColor("Background Color", Color) = (1,1,1,1)
		_Percentage("Percentage", Float) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _FillColor;
			fixed4 _BGColor;
			float _Percentage;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				//fixed4 col = tex2D(_MainTex, i.uv);
				//if (col.a == 0) {
				//	return col;
				//}
				if (uv.x > (0.5 - _Percentage/2) && uv.x < (0.5 + _Percentage / 2))
				{
					return _FillColor;
				}
				else
				{
					return _BGColor;
				}
			}
			ENDCG
		}
	}
}
