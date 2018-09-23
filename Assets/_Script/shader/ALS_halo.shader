Shader "ALS/ALS_halo" {
	Properties {
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		//Cull Off

		CGPROGRAM
			#pragma surface surf Lambert alpha:blend
			#pragma debug

			half4 _Color;
			//sampler2D _MainTex;

			struct Input {
				half2 uv_MainTex;
			};

			void surf (Input IN, inout SurfaceOutput o) {
				half2 c = IN.uv_MainTex - fixed2(.5, .5);
				o.Albedo = _Color.rgb;
				half d = dot(c, c);
				o.Alpha = d > .25 ? 0 : min(1.2 - d * 4, 1) * (abs(cos(d * 50 - _Time.w)) + .2) * _Color.a;

			}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/VertexLit"
}
