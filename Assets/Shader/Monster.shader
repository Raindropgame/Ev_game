Shader "Unlit/Monster"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _hitColor ("hitColor",COLOR) = (1,1,1,1)
		[PerRendererData] _isHit ("isHit",int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

		Pass
		{
			ZWrite off
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
			int _isHit;
			fixed4 _hitColor;
			
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
				col.rgb = lerp(col.rgb,_hitColor.rgb,_isHit);
				return col;
			}
			ENDCG
		}
	}
}
