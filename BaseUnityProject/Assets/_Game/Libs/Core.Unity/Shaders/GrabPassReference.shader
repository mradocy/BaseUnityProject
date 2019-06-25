Shader "Custom/GrabPassReference" {
    
    // fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
        _WaveFrequency("Wave Frequency", Float) = 5
        _WaveMagnitude("Wave Magnitude", Float) = .05
    }
    
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
            };
			
            // custom fields to be filled by the Shader Lab
			sampler2D _BackgroundTexture; // used by grabPass
            float _WaveFrequency;
			float _WaveMagnitude;
            
            // vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;
				OUT.vertex = UnityObjectToClipPos(v.vertex); // transforms model vertex positions based on the Unity camera
				OUT.uv = v.uv;

				OUT.grabPos = ComputeGrabScreenPos(OUT.vertex); // gets grab screen position

                return OUT;
            }
            
            // fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_Target {
                
				float4 grabPos = IN.grabPos;

				// sample: sine wavy
                grabPos.y += sin(IN.uv.x * 6.28 * _WaveFrequency + _Time.y) * _WaveMagnitude;

                // sample: inverts background color
                fixed4 bgColor = tex2Dproj(_BackgroundTexture, grabPos);
                fixed4 color = bgColor;
                color.rgb = 1 - color.rgb;

				return color;
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
}