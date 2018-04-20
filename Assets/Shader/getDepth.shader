Shader "Unlit/getDepth" {  
  SubShader
  {
			pass{
				ZTest Always Cull Off ZWrite Off

				CGPROGRAM
				#include "unitycg.cginc"
				#pragma vertex vert
				#pragma fragment frag

				struct v2f{
					fixed2 uv:TEXCOORD;
					float4 position:SV_POSITION;
				};

				sampler2D _CameraDepthTexture;

				v2f vert(appdata_img v)
				{
					v2f o;
					o.position = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.texcoord;
					return o;
				}

				float4 frag(v2f i):SV_TARGET
				{
					fixed2 uv;
					uv.x = i.uv.x;
					uv.y = 1 - i.uv.y;
					float depth = tex2D(_CameraDepthTexture,uv).r;
					return float4(depth,depth,depth,1);
				}

				ENDCG
			}		
  }

}
