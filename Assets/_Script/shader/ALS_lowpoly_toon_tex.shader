// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ALS/ALS_lowpoly_toon_tex" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MinLightMagnifier ("Min light magnifier", Range (0, 1)) = .5
		_MaxLightMagnifier ("Max light magnifier", Range (0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }

		Cull Back ZWrite On

		Pass {
			CGPROGRAM
			#pragma vertex vert
	        #pragma fragment frag

	        #include "UnityCG.cginc"

	        sampler2D _MainTex;
	        half4 _LightColor0;
	        half _MinLightMagnifier;
			half _MaxLightMagnifier;

	        struct appdata {
	        	float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
	        };

			struct v2f {
				float4 pos : SV_POSITION;
				half4 diff : COLOR;
				float2 uv : TEXCOORD0;
			};


			v2f vert(appdata i) {
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv = i.texcoord.xy;
				half3 n = UnityObjectToWorldNormal(i.normal);
	            o.diff = min(_MaxLightMagnifier, max(_MinLightMagnifier, dot(n, _WorldSpaceLightPos0.xyz))) * _LightColor0;
	            return o;
			}



			half4 frag (v2f i) : SV_Target {
				return tex2D(_MainTex, i.uv) * i.diff;
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
