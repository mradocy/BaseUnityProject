// A copy of the built-in UI/Default shader with fields for modifying hue, saturation, lightness.
Shader "Custom/UI/UIDefaultHSL" {

    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

        _HueOffset("Hue Offset", Range(0, 360)) = 0
        _SaturationMult("Saturation Multiplier", Range(0, 2)) = 1
        _LightnessMult("Lightness Multiplier", Range(0, 2)) = 1

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader {

        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass {
            Name "Default"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float _HueOffset;
            float _SaturationMult;
            float _LightnessMult;

            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v) {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.uv = TRANSFORM_TEX(v.uv, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;

#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
#endif

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
                if (abs(delta) < .00001f) { // same as 'if lightness is 0 or 1'
                    saturation = 0;
                } else {
                    saturation = delta / (1 - abs(2 * lightness - 1));
                }

                // apply offsets and multipliers
                hue = fmod(hue + _HueOffset, 360);
                saturation = saturate(saturation * _SaturationMult);
                lightness = saturate(lightness * _LightnessMult);

                // HSL -> RGB calculation: https://www.rapidtables.com/convert/color/hsl-to-rgb.html
                fixed c = (1 - abs(2 * lightness - 1)) * saturation;
                fixed x = c * (1 - abs(fmod(hue / 60, 2) - 1));
                fixed m = lightness - c / 2;
                fixed4 newColor = fixed4(m, m, m, color.a);
                if (hue < 60) {
                    newColor.r += c;
                    newColor.g += x;
                } else if (hue < 120) {
                    newColor.r += x;
                    newColor.g += c;
                } else if (hue < 180) {
                    newColor.g += c;
                    newColor.b += x;
                } else if (hue < 240) {
                    newColor.g += x;
                    newColor.b += c;
                } else if (hue < 300) {
                    newColor.r += x;
                    newColor.b += c;
                } else if (hue < 360) {
                    newColor.r += c;
                    newColor.b += x;
                }

                return newColor;
            }

            ENDCG
        }
    }
}