Shader "Self/grassWave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_scale ("scale",float) = 0
		_Speed ("Speed", float) = 0
	}
	SubShader
	{
		ZWrite off
		Cull off
		blend SrcAlpha OneMinusSrcAlpha

		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "DisableBatching"="True"}

		Pass
		{
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
			float _scale;
			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 newPos = v.vertex;
				newPos.x = v.vertex.x + (_scale * sin(_Time.y * _Speed + v.color.a * 10)) * lerp(0,1,v.uv.y - 0.35);  //a通道控制时间轴偏移

				o.vertex = UnityObjectToClipPos(newPos);
				o.color = v.color;

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * float4(i.color.rgb,1);
				return col;
			}
			ENDCG
		}
	}
}
