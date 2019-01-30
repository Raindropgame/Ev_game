Shader "Self/DoorShadow"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Color",COLOR) = (1,1,1,1)
		[PerRendererData] _Range ("Range", float) = 0.5
		[PerRendererData] _Wavelength ("Wavelength",float) = 0.5
		[PerRendererData] _Speed ("Speed",float) = 0.5
		[PerRendererData] _Surface ("Surface", float) = 0.5
		[PerRendererData] _fallSpeed ("fallSpeed",float) = 0
		_RelativeSpeed ("RelativeSpeed",Range(0,1)) = 1
		_Center_Y ("Center_Y",Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Transparent"}

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
				float2 uv : TEXCOORD1;
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
			float _fallSpeed;
			float _RelativeSpeed;
			float _Center_Y;
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 _vertex = v.vertex;
				_vertex.y = _Range * lerp(1,0.1,abs(_Center_Y - v.uv.x) / 0.5) * sin(v.vertex.x * _Wavelength + _Speed * _Time.y) + v.vertex.y;

				o.vertex = UnityObjectToClipPos(_vertex);
 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(float2(_fallSpeed, 0.0) * _Time.y);
				o.pos = v.vertex;
				o.pos2 = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = (tex2D(_MainTex, i.uv) * _RelativeSpeed + (1 - _RelativeSpeed)) * _Color;
				return col;
			}
			ENDCG
		}
	}
}
