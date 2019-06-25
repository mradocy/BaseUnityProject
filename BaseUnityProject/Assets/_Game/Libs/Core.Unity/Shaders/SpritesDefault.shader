// A copy of the built-in Sprites/Default shader, except the texture can be specified by the material.
// Do not generate mip-maps on the sprites.  This will cause weird artifacts with the texture wrapping.
Shader "Custom/SpritesDefault" {
    
    Properties {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [KeywordEnum(Repeat, Clamp)] _WrapX("Wrap X", Float) = 0
        [KeywordEnum(Repeat, Clamp)] _WrapY("Wrap Y", Float) = 0
    }
    
    SubShader {
        
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
            

            sampler2D _MainTex;
            float4 _MainTex_ST; // texture name + "_ST" will get the tiling/offset of the texture.  x: x tiling, y: y tiling, z: x offset, w: y offset.
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            fixed4 _Color;

            half _WrapX;
            half _WrapY;

            v2f vert(appdata_t IN) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                
                // applies tiling/offset from the given texture
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex); // unity macro that applies tiling/offset from the given texture: #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)

                OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
                
                return OUT;
            }
            
            
            
            fixed4 SampleSpriteTexture(float2 uv) {
                fixed4 color = tex2D(_MainTex, uv);
                
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
#endif
                return color;
            }
            
            fixed4 frag(v2f IN) : SV_Target {

                float2 uv = IN.uv;
                
                // wrap uv to be in [0, 1)
                if (_WrapX == 0) {
                    uv.x = frac(uv.x);
                }
                if (_WrapY == 0) {
                    uv.y = frac(uv.y);
                }
                
                fixed4 c = SampleSpriteTexture(uv) * IN.color;
                c.rgb *= c.a;
                return c;
            }

            ENDCG
        }
        
    }
}