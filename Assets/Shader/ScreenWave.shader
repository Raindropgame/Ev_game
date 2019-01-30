// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Self/ScreenWave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Pivot_x ("Pivot_x",float) = 0.5
		_Pivot_y ("Povit_y",float) = 0.5
		_Offset ("Offset",float) = 1
		_Scale ("Scale",float) = 10
		_limit ("limit",float) = 1

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			fixed _Pivot_x;
			fixed _Pivot_y;
			float _Offset;
			float _Scale;
			float _limit;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 dir = normalize(i.uv - fixed2(_Pivot_x * 1.78,_Pivot_y));
				float length = sqrt(pow((i.uv - fixed2(_Pivot_x,_Pivot_y)).x * 1.78,2) + pow((i.uv - fixed2(_Pivot_x,_Pivot_y)).y,2));
				fixed2 offset = dir * sin(length * _Scale) * _Offset;
				fixed a = (length - _limit)*10;
				if(a > 1)
				{
					a = 1;
				}
				offset = lerp(offset,fixed2(0,0),a) * step(_limit,length);
				fixed4 col = tex2D(_MainTex, i.uv + offset);
				
				return col;
			}
			ENDCG
		}
	}
}
