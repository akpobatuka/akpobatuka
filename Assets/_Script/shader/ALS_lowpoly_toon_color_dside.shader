// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ALS/ALS_lowpoly_toon_color_dside" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MinLightMagnifier ("Min light magnifier", Range (0, 1)) = .5
		_MaxLightMagnifier ("Max light magnifier", Range (0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }

		Cull Off ZWrite On

		Pass {
			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag

	        #include "UnityCG.cginc"

	        half4 _Color;
	        half4 _LightColor0;
	        half _MinLightMagnifier;
			half _MaxLightMagnifier;

	        struct appdata {
	        	float4 vertex : POSITION;
				float3 normal : NORMAL;
	        };

			struct v2f {
				float4 pos : SV_POSITION;
				half4 diff : COLOR;

			};


			v2f vert(appdata i) {
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex);
				half3 n = UnityObjectToWorldNormal(i.normal);
	            half nl = max(0, dot(n, _WorldSpaceLightPos0.xyz));
	            o.diff = min(_MaxLightMagnifier, max(_MinLightMagnifier, dot(n, _WorldSpaceLightPos0.xyz))) * _LightColor0;
	            return o;
			}



			half4 frag (v2f i) : SV_Target {
				return _Color * i.diff;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
