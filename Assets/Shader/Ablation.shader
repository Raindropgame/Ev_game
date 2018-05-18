Shader "Self/Ablation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTex",2D) = "white" {}
		_BurnScale ("BurnScale",Range(0,1)) = 0
		_BurnWidth ("BurnWidth",float) = 0
		_BurnColor ("BurnColor",color) = (1,1,1,1)
		_Color ("Color",COLOR) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

		Pass
		{
			zwrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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
				float2 Burn_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _BurnScale;
			float _BurnWidth;
			fixed4 _BurnColor;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.Burn_uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 burnTex = tex2D(_NoiseTex,i.Burn_uv);

				float isEx = step(_BurnScale,burnTex.r) * step(0.001,col.a); //是否已被燃烧

				fixed4 finalColor =isEx * lerp(_BurnColor,col, saturate((burnTex.r - _BurnScale) / _BurnWidth));

				return finalColor;
			}
			ENDCG
		}

	}

}
