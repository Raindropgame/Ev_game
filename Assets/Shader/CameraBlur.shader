Shader "Unlit/CameraBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BlurTex ("BlurTex", 2D) = "white" {}
		_focus ("focus", float) = 0
		_Color ("Color",COLOR) = (1,1,1,1)
	}
	SubShader
	{

		Pass
		{
			ZTest Always Cull off ZWrite Off
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
			sampler2D _BlurTex;
			sampler2D _DepthTex;
			float focus;
			sampler2D _CameraDepthTexture;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv;
				uv.x = i.uv.x;
				uv.y = 1 - i.uv.y;
				half depth = tex2D(_CameraDepthTexture, uv).r;
				half BlurDegree = abs(depth - focus) / 0.5f;
				fixed4 mainCol = tex2D(_MainTex,i.uv);
				fixed4 blurCol = tex2D(_BlurTex,uv);
				fixed4 col = lerp(mainCol, blurCol, pow(BlurDegree,0.5));
				fixed4 colDepth = lerp(fixed4(1,1,1,1),_Color,(depth - 0.5) * 2);
				fixed4 Fcol = step(0.5f, depth) * col * colDepth;
				fixed4 Bcol = step(depth, 0.5f) * col * lerp(fixed4(0,0,0,1),fixed4(1,1,1,1),depth * 2);
				return Fcol + Bcol;
				//return lerp(mainCol, blurCol, pow(BlurDegree,0.5)) * lerp(fixed4(1,1,1,1),_Color,(depth - 0.5) * 2);
			}
			ENDCG
		}
	}
}
