// allows hue/saturation/lightness adjustment on an image
Shader "Custom/HSLImage" {
    
    // fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _Hue("Hue", Range(0, 360)) = 0
        _Saturation("Saturation", Range(0, 2)) = 1
        _Lightness("Lightness", Range(0, 2)) = 1
    }
    
    SubShader {
        
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
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
            };
            
            // custom fields to be filled by the Shader Lab
            sampler2D _MainTex;
            float _Hue;
            float _Saturation;
            float _Lightness;
            
            // vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex); // transforms model vertex positions based on the Unity camera
                OUT.uv = v.uv;
                return OUT;
            }
            
            // fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_Target {

                fixed4 color = tex2D(_MainTex, IN.uv);
                
                // RGB -> HSL calculation: https://www.rapidtables.com/convert/color/rgb-to-hsl.html
                fixed r = color.r;
                fixed g = color.g;
                fixed b = color.b;
                fixed cMax = max(r, max(g, b));
                fixed cMin = min(r, min(g, b));
                fixed delta = cMax - cMin;

                // calculate hue (in [0, 360))
                float hue;
                if (abs(delta) < .00001f) {
                    hue = 0;
                } else if (cMax == color.r) {
                    hue = 60 * fmod((float)(g - b) / delta, 6);
                } else if (cMax == color.g) {
                    hue = 60 * ((float)(b - r) / delta + 2);
                } else {
                    hue = 60 * ((float)(r - g) / delta + 4);
                }

                // calculate lightness (in [0, 1])
                float lightness = (float)(cMax + cMin) / 2;

                // calculate saturation (in [0, 1])
                float saturation;
                if (abs(delta) < .00001f){ // same as 'if lightness is 0 or 1'
                    saturation = 0;
                } else {
                    saturation = delta / (1 - abs(2 * lightness - 1));
                }

                // apply offsets and multipliers
                hue = fmod(hue + _Hue, 360);
                saturation = saturate(saturation * _Saturation);
                lightness = saturate(lightness * _Lightness);

                // HSL -> RGB calculation: https://www.rapidtables.com/convert/color/hsl-to-rgb.html
                fixed c = (1 - abs(2 * lightness - 1)) * saturation;
                fixed x = c * (1 - abs(fmod(hue / 60, 2) - 1));
                fixed m = lightness - c / 2;
				fixed4 newColor = fixed4(m, m, m, 1);
                if (hue < 60){
                    newColor.r += c;
                    newColor.g += x;
                } else if (hue < 120){
                    newColor.r += x;
                    newColor.g += c;
                } else if (hue < 180){
                    newColor.g += c;
                    newColor.b += x;
                } else if (hue < 240){
                    newColor.g += x;
                    newColor.b += c;
                } else if (hue < 300){
                    newColor.r += x;
                    newColor.b += c;
                } else if (hue < 360){
                    newColor.r += c;
                    newColor.b += x;
                }

                return newColor;
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
}