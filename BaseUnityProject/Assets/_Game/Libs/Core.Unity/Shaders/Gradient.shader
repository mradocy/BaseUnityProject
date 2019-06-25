Shader "Custom/Gradient" {
    
    // fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
        _Color0("Color 0", Color) = (0, 0, 0, 1)
        _Color1("Color 1", Color) = (1, 1, 1, 1)
    }
    
    SubShader {

        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        // no culling or depth
        Cull Off
        ZWrite Off
        ZTest Always

        // alpha blending
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            // custom fields to be filled by the Shader Lab
            fixed4 _Color0;
            fixed4 _Color1;
            
            // vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex); // transforms model vertex positions based on the Unity camera
                OUT.uv = v.uv;

                return OUT;
            }
            
            // fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_Target {

                fixed4 color;
                color = lerp(_Color0, _Color1, IN.uv.x);
                
                return color;
                
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
    
}
