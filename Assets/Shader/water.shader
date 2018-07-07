Shader "Self/water"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Color",COLOR) = (1,1,1,1)
		[PerRendererData] _Range ("Range", float) = 0.5
		[PerRendererData] _Wavelength ("Wavelength",float) = 0.5
		[PerRendererData] _Speed ("Speed",float) = 0.5
		[PerRendererData] _Surface ("Surface", float) = 0.5
		_NormalMap ("NormalMap", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Transparent"}

		GrabPass{"_BGTex"}

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
				float4 pos2 : POSITION2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Range;
			float _Wavelength;
			float _Speed;
			float4 _Color;
			fixed _Surface;
			sampler2D _NormalMap;
			sampler2D _BGTex;			
			float4 _BGTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 _vertex = v.vertex;
				_vertex.y = _Range * sin(v.vertex.x * _Wavelength + _Speed * _Time.y) + v.vertex.y;

				o.vertex = UnityObjectToClipPos(_vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos = v.vertex;
				o.pos2 = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;
				fixed3 bump = UnpackNormal(tex2D(_NormalMap,i.uv));
				fixed2 offset = bump.xy * _BGTex_TexelSize.xy * 20;
				fixed3 BGCol = tex2D(_BGTex,i.pos2 + offset);
				fixed w;
				if(i.pos.y > _Surface)
				{
					w = lerp(0.5,0,i.pos.y - _Surface);
				}
				else
				{
					w = 0.5;
				}
				return fixed4(BGCol * w + (1 - w) * _Color,1);
			}
			ENDCG
		}
	}
}
