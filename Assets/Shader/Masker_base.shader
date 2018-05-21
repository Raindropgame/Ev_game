Shader "Unlit/Masker_base"
{
	Properties
	{
		_Alpha ("Alpha",Range(0,1)) = 1
	}

	SubShader
	{
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Pass
		{
			Tags {"LightMode" = "ForwardBase"}

			ZWrite off
			Blend zero one,one zero

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Alpha;

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

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0,0,0,_Alpha);
			}
			ENDCG
		}

		Pass
		{
			Tags {"LightMode" = "ForwardAdd"}

			ZWrite off
			blendop add,min
			Blend zero one,one one

			CGPROGRAM
			#pragma multi_compile_fwdadd
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "autoLight.cginc"
			#include "Lighting.cginc"

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
				UNITY_LIGHT_ATTENUATION(attenuation, i, i.worldPos.xyz);
				return fixed4(0,0,0,1 - attenuation);
			}
			ENDCG
		} 
	}
}
