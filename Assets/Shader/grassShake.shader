Shader "Self/grassShake"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_scale ("scale",float) = 0
		_Speed ("Speed", float) = 0
		_Pivot_y ("Pivot_y",Range(0,1)) = 0
		_Alpha ("Alpha",Range(0,1)) = 1
		_Offset ("Offset",float) = 0
		_NoiseTex ("NoiseTex",2D) = "white" {}
		[PerRendererData] _BurnScale ("BurnScale",Range(0,1)) = 0
		_BurnWidth ("BurnWidth",float) = 0
		[PerRendererData] _BurnColor ("BurnColor",color) = (1,1,1,1)
	}
	SubShader
	{
		ZWrite off
		Cull off
		blend SrcAlpha OneMinusSrcAlpha

		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "DisableBatching"="True"}

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
				float4 color:COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 color:COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _scale;
			float _Speed;
			float _Pivot_y;
			float _Alpha;
			float _Offset;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _BurnScale;
			float _BurnWidth;
			fixed4 _BurnColor;
					
			fixed4 burn(float4 col,float4 burnCol,fixed2 uv)
			{
				return lerp(_BurnColor,col, saturate((burnCol.r - _BurnScale) / _BurnWidth));
			}

			v2f vert (appdata v)
			{
				v2f o;
				float4 newPos = v.vertex;
				float random = UnityObjectToClipPos(v.vertex).x;
				newPos.x = v.vertex.x + (_scale * sin(_Time.y * _Speed + random) + _Offset) * lerp(0,1,v.uv.y - _Pivot_y);  //a通道控制时间轴偏移

				o.vertex = UnityObjectToClipPos(newPos);
				o.color = v.color;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _NoiseTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * float4(i.color.rgb,_Alpha);
				fixed4 burnTex = tex2D(_NoiseTex,i.uv2);
				float isEx = step(_BurnScale,burnTex.r) * step(0.001,col.a); //是否已被燃烧
				col = isEx * burn(col,burnTex,i.uv);
				return col;
			}
			ENDCG
		}
	}
}
