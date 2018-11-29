Shader "Self/GemOutline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_OutlineCol ("OutlineCol",COLOR) = (1,1,1,1)
		_Width ("Width",Range(0,20)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}

		Pass
		{
			ZWrite off cull off
			blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv[9] : TEXCOORD0;
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			fixed4 _OutlineCol;
			float _Width;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float2 uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv[0] = uv + _MainTex_TexelSize * fixed2(-1,1) * _Width;
				o.uv[1] = uv + _MainTex_TexelSize * fixed2(0,1) * _Width;
				o.uv[2] = uv + _MainTex_TexelSize * fixed2(1,1) * _Width;
				o.uv[3] = uv + _MainTex_TexelSize * fixed2(-1,0) * _Width;
				o.uv[4] = uv + _MainTex_TexelSize * fixed2(0,0) * _Width;
				o.uv[5] = uv + _MainTex_TexelSize * fixed2(1,0) * _Width;
				o.uv[6] = uv + _MainTex_TexelSize * fixed2(-1,-1) * _Width;
				o.uv[7] = uv + _MainTex_TexelSize * fixed2(0,-1) * _Width;
				o.uv[8] = uv + _MainTex_TexelSize * fixed2(1,-1) * _Width;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 TexCol = tex2D(_MainTex, i.uv[4]); 
				float a = 0;
				for(int t = 0;t<9;t++)
				{
					a =a + tex2D(_MainTex, i.uv[t]).a;
				}
				a = pow(a / 9,3);
				float4 finalCol;
				if(TexCol.a > 0.9)
				{
					finalCol = TexCol;
				}
				else
				{
					clip(a - 0.01);
					finalCol =  lerp(_OutlineCol,TexCol,a);
				}
				return finalCol * i.color;
			}
			ENDCG
		}
	}
}