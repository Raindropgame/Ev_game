Shader "Unlit/Monster"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _hitColor ("hitColor",COLOR) = (1,1,1,1)
		[PerRendererData] _isHit ("isHit",int) = 0
		[Header(Froze)]
		[Space]
		[PerRendererData] _isFroze ("isFroze",float) = 0
		_frozeTex ("frozeTex",2D) = "white" {}
		_frozeColor ("frozeColor",COLOR) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

		Pass
		{
			ZWrite on
			Cull off
			blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag		
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color:COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				fixed4 color:COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			int _isHit;
			fixed4 _hitColor;
			sampler2D _frozeTex;
			float4 _frozeTex_ST;
			float4 _frozeColor;
			float _isFroze;

			float4 froze(fixed2 uv,float4 color)
			{
				float4 frozeCol = tex2D(_frozeTex,uv) * 1.6f;
				float4 col = frozeCol * _frozeColor.a + color * (1 - _frozeColor.a);
				col.a = color.a;
				return col;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _frozeTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex, i.uv) * i.color;
				float4 col = mainCol;
				clip(col.a - 0.001);
				col.rgb = lerp(col.rgb,_hitColor * 0.7 + 0.3 * col.rgb.rgb,_isHit);  //被击

				//冰冻
				if(_isFroze > 0.5)
				{
					col = froze(i.uv2,col);
					col.rgb = lerp(col.rgb,_frozeColor.rgb,step((mainCol.r + mainCol.g + mainCol.b),0.1));
				}
				return col;
			}
			ENDCG
		}
	}
}
