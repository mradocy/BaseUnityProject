Shader "Custom/ImageEffectReference" {
    
    // fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
        _MainTex("Texture", 2D) = "white" {}
        _WaveFrequency("Wave Frequency", Float) = 5
        _WaveMagnitude("Wave Magnitude", Float) = .05
        _Color("Color", Color) = (1,1,1,1)
        _ColorAmount("Color Amount", Range(0, 1)) = 1.0
        _MeshWidth("Mesh Width", Float) = 0
        _MeshHeight("Mesh Height", Float) = 0
        _MeshPxWidth("Mesh Pixel Width", Float) = 0
        _MeshPxHeight("Mesh Pixel Height", Float) = 0
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
            
            // custom fields to be filled by Unity's Shader Lab
            sampler2D _MainTex;
            float4 _MainTex_ST; // texture name + "_ST" will get the tiling/offset of the texture.  x: x tiling, y: y tiling, z: x offset, w: y offset.
            float4 _MainTex_TexelSize; // texture name + "_TexelSize" will get size of the texture.  x: 1/width, y: 1/height, z: width, w: height.
            float _WaveFrequency;
            float _WaveMagnitude;
            fixed4 _Color;
            float _ColorAmount;
            float _MeshWidth;
            float _MeshHeight;
            float _MeshPxWidth;
            float _MeshPxHeight;
            
            // vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, v.vertex); // transforms model vertex positions based on the Unity camera
                OUT.uv = v.uv;

                // sample: set texture tiling so that it won't appear stretched
                _MainTex_ST.x = _MeshPxWidth * _MainTex_TexelSize.x;
                _MainTex_ST.y = _MeshPxHeight * _MainTex_TexelSize.y;

                // sample: applies tiling/offset from the given texture
                OUT.uv = TRANSFORM_TEX(v.uv, _MainTex); // unity macro that applies tiling/offset from the given texture: #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)

                return OUT;
            }
            
            // fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_Target {
                float2 uv = IN.uv;
                uv = frac(uv); // wraps uv to be in [0, 1)
                
                // sample: invert color of texture
                //fixed4 color = tex2D(_MainTex, uv); // gets color from the texture at the given uv
                //color.rgb = 1 - color.rgb;
                
                // sample: sine wavy
                float2 sinUV = uv;
                sinUV.x += sin(sinUV.y * 6.28 * _WaveFrequency + _Time.y) * _WaveMagnitude;
                fixed4 color = tex2D(_MainTex, sinUV);

                // sample: fade to color
                color.rgb = lerp(color.rgb, _Color.rgb, _ColorAmount);

                return color;
                
            }
            
            ENDCG
        }
        
    }
    
    Fallback "Sprites/Default"
    
}

// Cg functions reference: http://developer.download.nvidia.com/cg/index_stdlib.html

// To change amount in code:
//spriteRenderer.material.SetFloat("_ColorAmount", 0.5f);
// To change color in code:
//spriteRenderer.material.SetColor("_Color", Color.green);

