Shader "Self/Portal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTex",2D) = "white" {}
		_offsetScale ("offsetScale",float) = 1
		_color ("color",color) = (1,1,1,1)
		_color2 ("color2",color) = (1,1,1,1)
		alpha ("Angle",float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

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
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 color : COLOR0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _offsetScale;
			float4 _color;
			float4 _color2;
			float alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv,_NoiseTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv2 - 0.5;
				float2 uv_offset;
				alpha = _Time.x * alpha;
				uv_offset.x = uv.x * cos(alpha) - uv.y * sin(alpha);
				uv_offset.y = uv.x * sin(alpha) + uv.y * cos(alpha);
				uv_offset += 0.5; 
				fixed2 offset = (tex2D(_NoiseTex,uv_offset + _Time.x * 5).rg * 2 - 1) * _offsetScale;
				fixed4 col = tex2D(_MainTex, i.uv + offset) * 1.2f * i.color;
				col *= lerp(_color,_color2,pow(col.a,1));
				return col;
			}
			ENDCG
		}
	}
}
