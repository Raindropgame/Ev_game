Shader "Self/Vine"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale ("Scale",Range(0,1)) = 1
		_Total ("Total",float) = 1
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			blend SrcAlpha OneMinusSrcAlpha
			

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR1; 
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Scale;
			float _Total;
			
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
				fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb = col.rgb * _Scale + i.color * (_Total - _Scale);
				return col;
			}
			ENDCG
		}
	}
}
