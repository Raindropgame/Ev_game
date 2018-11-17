Shader "Self/Thorns"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LighterScale ("LighterScale",float) = 1
		[PerRendererData] _Speed ("Speed",float) = 1
		[PerRendererData] _Scale ("Scale",float) = 0
		[PerRendererData] _Pivot_x ("Pivot",float) = 0
		[PerRendererData] _Pivot_y ("Pivot",float) = 0

	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }

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
				float4 color:COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color:COLOR1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _LighterScale;
			float _Scale;
			float _Pivot_x;
			float _Pivot_y;
			float _Speed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 offset = distance(i.uv,float2(_Pivot_x,_Pivot_y)) * _Scale * sin(_Time.y * 1 / _Speed + i.color.a) * (i.uv - float2(_Pivot_x,_Pivot_y));
				fixed4 col = tex2D(_MainTex, i.uv + offset) * _LighterScale * float4(i.color.rgb,1.0);
				return col;
			}
			ENDCG
		}
	}
}
