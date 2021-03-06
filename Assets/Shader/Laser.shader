﻿Shader "Self/Laser"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTex",2D) = "white" {}
		_Speed ("Speed",float) = 1
		_Width ("Width",Range(0,1)) = 0.5
		_FlashCycle ("FlashCycle",float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass
		{
			ZWrite Off
			Cull Off
			blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color:COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			fixed _Speed;
			fixed _Width;
			fixed _FlashCycle;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv,_NoiseTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{				
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed noise = tex2D(_NoiseTex, i.uv2 + fixed2(-_Time.y * _Speed,0));
				col.a *= (noise * 1.7);
				col = lerp(col,  fixed4(i.color.rgb * noise * lerp(4,10,sin(_Time.y * _FlashCycle) * 0.5 + 0.5),1),  step(abs(i.uv.y - 0.5) * 2,_Width));
				return col * i.color;
			}
			ENDCG
		}
	}
}