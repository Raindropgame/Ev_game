Shader "Self/Sprite_Lightning"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Random_x ("Random_x",float) = 0
		[PerRendererData] _Random_y ("Random_y",float) = 0
		_NoiseTex ("NoiseTex",2D) = "white" {}
		_LightTex ("LightTex",2D) = "white" {}
		_Color ("Color",color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass
		{
			Cull Off
			Lighting Off
			Ztest on
			blend SrcAlpha OneMinusSrcAlpha

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
			sampler2D _LightTex;
			fixed4 _Color;
			sampler2D _NoiseTex;
			float _Random_x;
			float _Random_y;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 lightCol = tex2D(_LightTex,i.uv + fixed2(_Random_x,_Random_y)) * _Color;
				return fixed4(lightCol.rgb * lightCol.a + col.rgb * (1 - lightCol.a),col.a);
			}
			ENDCG
		}
	}
}
