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
            #include "Assets/_Game/Libs/Core.Unity/Shaders/ColorUtils.cginc"

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
                
                // get hsl
                fixed3 hsl = RGBtoHSL(color);
                float hue = hsl.x * 360;
                float saturation = hsl.y;
                float lightness = hsl.z;

                // apply offsets and multipliers
                hue = fmod(hue + _Hue, 360);
                saturation = saturate(saturation * _Saturation);
                lightness = saturate(lightness * _Lightness);

                // convert back to color
                hsl = fixed3(hue / 360, saturation, lightness);
                fixed4 newColor = fixed4(HSLtoRGB(hsl), 1);

                return newColor;
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
}