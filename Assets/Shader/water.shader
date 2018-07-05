Shader "Self/water"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Color",COLOR) = (1,1,1,1)
		[PerRendererData] _Alpha ("Alpha",Range(0,1)) = 1
		[PerRendererData] _Range ("Range", float) = 0.5
		[PerRendererData] _Wavelength ("Wavelength",float) = 0.5
		[PerRendererData] _Speed ("Speed",float) = 0.5
		[PerRendererData] _Surface ("Surface", float) = 0.5
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
				float4 pos : POSITION1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Range;
			float _Wavelength;
			float _Speed;
			float _Alpha;
			float4 _Color;
			fixed _Surface;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 _vertex = v.vertex;
				_vertex.y = _Range * sin(v.vertex.x * _Wavelength + _Speed * _Time.y) + v.vertex.y;

				o.vertex = UnityObjectToClipPos(_vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				if(i.pos.y > _Surface)
				{
					col.a = _Alpha * col.a * 1.5;
				}
				else
				{
					col.a = _Alpha * col.a;
				}
				return fixed4(col.rgb,col.a);
			}
			ENDCG
		}
	}
}
