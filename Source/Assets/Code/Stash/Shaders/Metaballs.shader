// Following this tutorial: http://patomkin.com/blog/metaball-tutorial/
// This replaces the custom shader the tutorial mentions.

Shader "Custom/Metaballs" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    _Color1("Color 1", Color) = (1,1,1,1)
    _Alpha1("Alpha 1", Range(0,1)) = 0.7
    _Color2("Color 2", Color) = (1,1,1,1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            fixed _Cutoff;
            half4 _Color1;
            fixed _Alpha1;
            half4 _Color2;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
                
                // alpha cutoff
                clip(col.a - _Cutoff);

                // apply color based on alpha level
                if (col.a < _Alpha1) {
                    col = _Color1;
                } else {
                    col = _Color2;
                }
                
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}
