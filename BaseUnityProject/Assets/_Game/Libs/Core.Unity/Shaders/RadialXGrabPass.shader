// GrabPass transforming cartesian into radial coordinates, where dy is the radius and dx is the arc length
Shader "Custom/RadialXGrabPass" {
    
    // fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
        _CenterU("Circle Center U", Float) = 0.5
        _CenterV("Circle Center V", Float) = -3
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
			
            // _CenterUstom fields to be filled by the Shader Lab
			sampler2D _BackgroundTexture; // used by grabPass
            float _CenterU;
            float _CenterV;
            
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
                float2 uv = IN.uv;

                float r = sqrt((uv.x - _CenterU) * (uv.x - _CenterU) + (uv.y - _CenterV) * (uv.y - _CenterV));
                float theta = atan2(uv.x - _CenterU, uv.y - _CenterV); // note that 0 radians is up in this case

				float4 grabPos = IN.grabPos;
                grabPos.y = r + _CenterV;
                grabPos.y = 1 - grabPos.y; // is inverted otherwise
                grabPos.x = r * theta + _CenterU;

                fixed4 color = tex2Dproj(_BackgroundTexture, grabPos);

				return color;
            }
            
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}