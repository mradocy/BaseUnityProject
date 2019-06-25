// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GrabPassInvert" {
    
    // fields that can be accessed in Unity
    //Properties {
    //    _MainTex("Texture", 2D) = "white" {}
    //}
    
    SubShader {
        
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
		// grab the screen behind the object into _BackgroundTexture
		GrabPass {
			"_BackgroundTexture"
		}
	    
        Pass {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            // struct to be filled with data from the object
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            // "vertex to fragment", struct used to pass information to the fragment function
            struct v2f {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                //float2 uv : TEXCOORD0;
                //float4 vertex : SV_POSITION;
            };
			
			sampler2D _BackgroundTexture; // used by grabPass
            
            // custom fields to be filled by the Shader Lab
            //sampler2D _MainTex;
            
            // vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;				
				// Unity functions to get grabPos
				OUT.pos = UnityObjectToClipPos(v.vertex);
				OUT.grabPos = ComputeGrabScreenPos(OUT.pos);
                return OUT;
            }
            
            // fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_Target {
                
                // sample: inverts background color
                fixed4 bgColor = tex2Dproj(_BackgroundTexture, IN.grabPos);
                fixed4 color = bgColor;
                color.rgb = 1 - color.rgb;
				return color;
				
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
    
}
