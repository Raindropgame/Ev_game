Shader "Unlit/map"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color",COLOR) = (0,0,0,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "RenderType" = "Transparent"}

		Pass
		{
			zwrite Off
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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				return col * _Color;
			}
			ENDCG
		}

				Pass {  
			Tags { "LightMode"="ShadowCaster" }  
			CGPROGRAM  
			#pragma vertex vert  
			#pragma fragment frag  
			#pragma multi_compile_shadowcaster  
			#include "UnityCG.cginc"  
  
			sampler2D _MainTex;  
  
			struct v2f{  
				V2F_SHADOW_CASTER;  
				float2 uv:TEXCOORD2;  
			};  
  
			v2f vert(appdata_base v){  
				v2f o;  
				o.uv = v.texcoord.xy;  
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);  
				return o;  
			}  
  
			float4 frag( v2f i ) : SV_Target  
			{  
				fixed alpha = tex2D(_MainTex, i.uv).a;  
				clip(alpha - 0.2);  
				SHADOW_CASTER_FRAGMENT(i)  
			}  
  
			ENDCG  
		} 
	}

}
