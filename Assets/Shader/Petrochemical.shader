Shader "Self/Petrochemical"
{
		Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tex_stone ("Tex_stone",2D) = "white" {}
		_LineColor ("LineColor",COLOR) = (0,0,0,0)
		_Scale ("Scale",Range(0,1)) = 0.3
		_DarkScale ("DarkScale",float) = 1
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Tex_stone;
			float4 _Tex_stone_ST;
			fixed4 _LineColor;
			fixed _Scale;
			fixed _DarkScale;
			
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
				fixed4 col_stone = tex2D(_Tex_stone,i.uv);
				fixed4 finalCol = fixed4((col.rgb * 0.2 + col_stone.rgb * 0.8) * _DarkScale,col.a);
				finalCol.rgb = lerp(_LineColor.rgb ,finalCol.rgb ,step(_Scale,col.r + col.g + col.b));
				clip(finalCol.a - 0.001f);
				return finalCol;
			}
			ENDCG
		}
	}
}
