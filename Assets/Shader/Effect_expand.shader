Shader "Self/Effect_expand"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask ("Mask",2D) = "white" {}
		_bottom ("bottom",Range(0,1)) = 0
		_expand ("expand",float) = 1
		_Color ("Color",COLOR) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass
		{
			ZWrite Off
			Cull Off
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
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Mask;
			float4 _Mask_ST;
			float _bottom;
			float _expand;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _Mask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float distance = length(i.uv - fixed2(0.5,0.5));
				float a = _Time.y * 3;
				fixed2 uv = ((i.uv - fixed2(0.5,0.5)) * fixed2(cos(a) + sin(a),-sin(a) + cos(a)) * lerp(_expand,1,distance /0.5)) + fixed2(0.5,0.5);
				fixed4 col = tex2D(_MainTex, uv);
				float mask = tex2D(_Mask,i.uv2).r;
				clip(-col.r + _bottom);
				col *= 3;
				col = saturate(col);
				col.a = mask;
				col *= _Color;
				return col;
			}
			ENDCG
		}
	}
}
