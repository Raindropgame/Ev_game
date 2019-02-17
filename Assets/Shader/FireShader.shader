Shader "Unlit/FireShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DistortTex ("DistortTex",2D) = "white" {}
		_Mask ("Mask",2D) = "white" {}
		_Speed_Y ("Speed_Y",float) = 0
		_DistortStrength ("DistortStrength",Range(0,1)) = 0
		_GradientTimes ("GradientTimes",float) = 1
		_Height ("Height",float) = 0
		_ColorTop ("ColorTop",COLOR) = (1,1,1,1)
		_ColorBottom ("ColorBottom",COLOR) = (1,1,1,1)
		_Rim ("Rim",Range(0,1)) = 1
		_Discard ("Discard",Range(0,1)) = 1
		_RimWidth ("RimWidth",Range(0,1)) = 1
		_Alpha ("Alpha",Range(0,1)) = 1
		[PerRendererData] _Random ("Random",float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100

		Pass
		{
			ZWrite off
			Cull off
			Blend SrcAlpha OneMinusSrcAlpha
			
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
				float2 uv3 : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DistortTex;
			float4 _DistortTex_ST;
			float _Speed_Y;
			float _DistortStrength;
			float _GradientTimes;
			float _Height;
			fixed4 _ColorTop;
			fixed4 _ColorBottom;
			float _Rim;
			float _Discard;
			sampler2D _Mask;
			float4 _Mask_ST;
			fixed _RimWidth;
			fixed _Alpha;
			fixed _Random;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv, _DistortTex);
				o.uv3 = TRANSFORM_TEX(v.uv,_Mask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				_ColorBottom = lerp(fixed4(1,1,1,1),_ColorBottom,i.uv2.y + 0.3);
				fixed2 offset = (tex2D(_DistortTex, i.uv2).rg * 2 - 1) * _DistortStrength;
				fixed4 mainCol = tex2D(_MainTex, fixed2(i.uv.x,i.uv.y + _Speed_Y * (_Time.y + _Random)) + offset);
				fixed4 col = mainCol;
				float gradient = pow(lerp(1,0,i.uv2.y + _Height),_GradientTimes);
				col *= gradient;
				col = saturate(col);
				if(col.r < _Discard)
				{	
					clip(-0.1);
				}
				float alpha = tex2D(_Mask,i.uv3 + (mainCol.rg * 2 - 1) * 0.1).r;
				if(alpha < 0.1)
				{
					clip(-0.1);
				}
				float t = col.r - _Rim;
				col = lerp(_ColorTop,_ColorBottom,step(0,t));
				if(alpha > 0.1 && alpha < lerp(0,_RimWidth,i.uv2.y))
				{
					col = _ColorTop;
				}
				col.a = _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
