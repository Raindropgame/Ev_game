// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Self/Lightning"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Scale ("Scale",float) = 100
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
				fixed4 col : COLOR0;
			};

			struct v2f
			{
				float4 pos : POSITION1;
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 col : COLOR1;
			};

			float _Scale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.pos = v.vertex;
				o.col = v.col;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float3 vect = (i.pos.xyz - float3(0,0,0));
				float distance = pow(vect.x,2) + pow(vect.y,2);
				clip(_Scale - distance);
				fixed4 col = tex2D(_MainTex, i.uv);
				
				return col * i.col;
			}
			ENDCG
		}
	}
}
