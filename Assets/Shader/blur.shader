Shader "Unlit/blur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" 
	}
	SubShader
	{

		

		Pass
		{
			ZTest Always Cull Off ZWrite Off
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
				float2 uv[5] : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv[0] = v.uv + _MainTex_TexelSize.xy * fixed2(0,-2);
				o.uv[1] = v.uv + _MainTex_TexelSize.xy * fixed2(0,-1);
				o.uv[2] = v.uv + _MainTex_TexelSize.xy * fixed2(0,0);
				o.uv[3] = v.uv + _MainTex_TexelSize.xy * fixed2(0,1);
				o.uv[4] = v.uv + _MainTex_TexelSize.xy * fixed2(0,2);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed weight[5] = {0.055,0.244,0.403,0.244,0.055};
				half4 col = half4(0,0,0,0);
				for(int t = 0;t<5;t++)
				{
					col += tex2D(_MainTex, i.uv[t]) * weight[t];
				}
				return col;
			}
			ENDCG
		}

		Pass
		{
			ZTest Always Cull Off ZWrite Off
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
				float2 uv[5] : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv[0] = v.uv + _MainTex_TexelSize.xy * fixed2(-2,0);
				o.uv[1] = v.uv + _MainTex_TexelSize.xy * fixed2(-1,0);
				o.uv[2] = v.uv + _MainTex_TexelSize.xy * fixed2(0,0);
				o.uv[3] = v.uv + _MainTex_TexelSize.xy * fixed2(1,0);
				o.uv[4] = v.uv + _MainTex_TexelSize.xy * fixed2(2,0);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed weight[5] = {0.055,0.244,0.403,0.244,0.055};
				half4 col = half4(0,0,0,0);
				for(int t = 0;t<5;t++)
				{
					col += tex2D(_MainTex, i.uv[t]) * weight[t];
				}
				return col;
			}
			ENDCG
		}
	}
}
