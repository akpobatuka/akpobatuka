// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ALS/ALS_lowpoly_edge" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_EdgeColor ("Edge Color", Color) = (0,0,0,1)
		_EdgeStep ("Edge step", Range (0, 10)) = 3
		_DeltaDepth ("Delta depth", Range (0, 1)) = 0.1
		_DeltaNorm ("Delta norm", Range (-1, 1)) = 0.1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }

		//Cull Off ZWrite Off ZTest Always
		//Cull Back ZWrite Off

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				uniform float4 _MainTex_TexelSize;
				sampler2D _MainTex;
				half4 _EdgeColor;
				half _EdgeStep;
				half _DeltaDepth;
				half _DeltaNorm;
				sampler2D _CameraDepthNormalsTexture;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv[4] : TEXCOORD0;
				};

				v2f vert (appdata_img i) {
					v2f o;
					o.pos = UnityObjectToClipPos(i.vertex);

					float2 uv = i.texcoord.xy;
					o.uv[0] = uv;
					
					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
					#endif

					o.uv[1] = uv;
					o.uv[2] = _MainTex_TexelSize.xy * _EdgeStep + uv;
					o.uv[3] = float2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y) * _EdgeStep + uv;

					return o;
				}

				//inline half check_depth(float2 zw0, float2 zw1) { // DecodeFloatRG
				//	return abs((zw0.y - zw1.y) / 255.0 + zw0.x - zw1.x) > _DeltaDepth ? 1.0 : 0.0;
				//}

				half4 frag (v2f i) : SV_Target {
					float4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
					float4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
					float4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
					
					// depth check
					//if (check_depth(sample1.zw, sample2.zw) || check_depth(sample1.zw, sample3.zw))
					if (abs(sample1.z - sample2.z) > _DeltaDepth || abs(sample1.z - sample3.z) > _DeltaDepth)
						return _EdgeColor;
					
					// norm check
					//if (dot(DecodeViewNormalStereo(sample1), DecodeViewNormalStereo(sample2)) < _DeltaNorm
					//	||
					//	dot(DecodeViewNormalStereo(sample1), DecodeViewNormalStereo(sample3)) < _DeltaNorm
					//	)

					half2 diff = max(abs(sample1.xy - sample2.xy), abs(sample1.xy - sample3.xy));
					if (diff.x + diff.y > _DeltaNorm)
						return _EdgeColor;
					
					return tex2D(_MainTex, i.uv[0]);
				}


			ENDCG





		} // Pass 1
	} // SubShader

	Fallback "Mobile/Diffuse"
}