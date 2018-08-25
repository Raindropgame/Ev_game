Shader "Self/LeafWave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Scale ("Scale",float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

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
				float4 color : COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 vertex = UnityObjectToClipPos(v.vertex) ;
				o.vertex = vertex + _Scale * fixed4(1,-0.2,0,0) * pow(distance(v.vertex , fixed4(0,0,0,1)),2);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * i.color;
				return col;
			}
			ENDCG
		}
	}
}
