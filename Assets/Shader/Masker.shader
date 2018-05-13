// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Transparent/Masker"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color",COLOR) = (1,1,1,1)
		_Distance ("Distance",float) = 10
	}
	SubShader
	{
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0,0,0,0);
			}
			ENDCG
		}

		Pass
		{
			Tags {"LightMode" = "ForwardAdd"}

			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma multi_compile_fwdadd
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "autoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float _Distance;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 worldLightPos = _WorldSpaceLightPos0.xyz;
				fixed3 Light = worldLightPos - i.worldPos;
				fixed distance = dot(Light.xy,Light.xy);
				return fixed4(col.xyz * _Color,smoothstep(0,1,sqrt(distance)/_Distance));
			}
			ENDCG
		} 
	}
}
