
Shader "Custom/SpritesPerlinNoiseBurn" {
    
    Properties {
		// SpritesDefault
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [KeywordEnum(Repeat, Clamp)] _WrapX("Wrap X", Float) = 0
        [KeywordEnum(Repeat, Clamp)] _WrapY("Wrap Y", Float) = 0
		// Burn
		_BurnThreshold("Burn Threshold", Range(0, 1)) = .5
		_BurnInterval("Burn Interval", Range(0, 1)) = .1
		_BurnColor("Burn Color", Color) = (1, 0, 0, 1)
		// Perlin Noise
		_pnOffsetX("PN OffsetX", Float) = 0.0
        _pnOffsetY("PN OffsetY", Float) = 0.0
        _pnOctaves("PN Octaves", Int) = 7
        _pnLacunarity("PN Lacunarity", Range(1.0, 5.0)) = 2
        _pnGain("PN Gain", Range(0.0, 1.0)) = 0.5
        _pnValue("PN Value", Range(-2.0, 2.0)) = 0.0
        _pnAmplitude("PN Amplitude", Range(0.0, 5.0)) = 1.5
        _pnFrequency("PN Frequency", Range(0.0, 6.0)) = 2.0
        _pnPower("PN Power", Range(0.1, 5.0)) = 1.0
        _pnScale("PN Scale", Float) = 1.0
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
                float2 uv : TEXCOORD0;
            };
            
			// SpritesDefault fields
            sampler2D _MainTex;
            float4 _MainTex_ST; // texture name + "_ST" will get the tiling/offset of the texture.  x: x tiling, y: y tiling, z: x offset, w: y offset.
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            fixed4 _Color;
			half _WrapX;
            half _WrapY;

			// Burn fields
			float _BurnThreshold;
			float _BurnInterval;
			fixed4 _BurnColor;

			// Perlin Noise fields
			float _pnOctaves, _pnLacunarity, _pnGain, _pnValue, _pnAmplitude, _pnFrequency, _pnOffsetX, _pnOffsetY, _pnPower, _pnScale;

			// gets color for a given uv from the main texture
			fixed4 SampleSpriteTexture(float2 uv) {
                fixed4 color = tex2D(_MainTex, uv);
                
#if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
#endif
				
                return color;
            }

			// calculates perlin noise value (in [0, 1]) for the given uv point
            float PerlinNoise2D(float2 uv) {
                uv = uv * _pnScale + float2(_pnOffsetX, _pnOffsetY);
                for (int i = 0; i < _pnOctaves; i++) {
                    float2 i = floor(uv * _pnFrequency);
                    float2 f = frac(uv * _pnFrequency);      
                    float2 t = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
                    float2 a = i + float2(0.0, 0.0);
                    float2 b = i + float2(1.0, 0.0);
                    float2 c = i + float2(0.0, 1.0);
                    float2 d = i + float2(1.0, 1.0);
                    a = -1.0 + 2.0 * frac(sin(float2(dot(a, float2(127.1, 311.7)), dot(a, float2(269.5, 183.3)))) * 43758.5453123);
                    b = -1.0 + 2.0 * frac(sin(float2(dot(b, float2(127.1, 311.7)), dot(b, float2(269.5, 183.3)))) * 43758.5453123);
                    c = -1.0 + 2.0 * frac(sin(float2(dot(c, float2(127.1, 311.7)), dot(c, float2(269.5, 183.3)))) * 43758.5453123);
                    d = -1.0 + 2.0 * frac(sin(float2(dot(d, float2(127.1, 311.7)), dot(d, float2(269.5, 183.3)))) * 43758.5453123);
                    float A = dot(a, f - float2(0.0, 0.0));
                    float B = dot(b, f - float2(1.0, 0.0));
                    float C = dot(c, f - float2(0.0, 1.0));
                    float D = dot(d, f - float2(1.0, 1.0));
                    float noise = (lerp(lerp(A, B, t.x), lerp(C, D, t.x), t.y));              
                    _pnValue += _pnAmplitude * noise;
                    _pnFrequency *= _pnLacunarity;
                    _pnAmplitude *= _pnGain;
                }
                _pnValue = clamp(_pnValue, -1.0, 1.0);
                return pow(_pnValue * 0.5 + 0.5, _pnPower);
            }


            v2f vert(appdata_t IN) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                
                // applies tiling/offset from the given texture
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex); // unity macro that applies tiling/offset from the given texture: #define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)

#ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
                
                return OUT;
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

				fixed4 c = SampleSpriteTexture(uv);

				// Perlin Noise Burn
				float pn = PerlinNoise2D(uv);
				if (pn < _BurnThreshold) {
					c.a = 0;
				} else if (pn < _BurnThreshold + _BurnInterval) {
				    float burnAmount = 1 - (pn - _BurnThreshold) / _BurnInterval;
				    c.rgb = lerp(c.rgb, _BurnColor.rgb, burnAmount);
				}

				// Sprites Default Color
                c *= _Color;
                c.rgb *= c.a;
                return c;
            }



            ENDCG
        }
        
    }
}