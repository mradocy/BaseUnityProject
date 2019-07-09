//2D Gradient Perlin Noise
//https://github.com/przemyslawzaworski/Unity3D-CG-programming
 
Shader "Custom/PerlinNoise2D" {
	
	// fields that can be accessed in Unity.  More info: https://docs.unity3d.com/Manual/SL-PropertiesInPrograms.html
    Properties {
		// Perlin Noise
        _OffsetX("OffsetX", Float) = 0.0
        _OffsetY("OffsetY", Float) = 0.0      
        _Octaves("Octaves", Int) = 7
        _Lacunarity("Lacunarity", Range(1.0, 5.0)) = 2
        _Gain("Gain", Range(0.0, 1.0)) = 0.5
        _Value("Value", Range(-2.0, 2.0)) = 0.0
        _Amplitude("Amplitude", Range(0.0, 5.0)) = 1.5
        _Frequency("Frequency", Range(0.0, 6.0)) = 2.0
        _Power("Power", Range(0.1, 5.0)) = 1.0
        _Scale("Scale", Float) = 1.0

		// Other
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)  
    }

    Subshader {

        Pass {
		    
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
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
 
			// Perlin Noise custom fields to be filled by the Shader Lab
            float _Octaves, _Lacunarity, _Gain, _Value, _Amplitude, _Frequency, _OffsetX, _OffsetY, _Power, _Scale;

			// Other custom fields
            float4 _Color;
            
			// calculates perlin noise value (in [0, 1]) for the given uv point
            float PerlinNoise2D(float2 uv) {
                uv = uv * _Scale + float2(_OffsetX, _OffsetY);
                for (int i = 0; i < _Octaves; i++) {
                    float2 i = floor(uv * _Frequency);
                    float2 f = frac(uv * _Frequency);      
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
                    _Value += _Amplitude * noise;
                    _Frequency *= _Lacunarity;
                    _Amplitude *= _Gain;
                }
                _Value = clamp(_Value, -1.0, 1.0);
                return pow(_Value * 0.5 + 0.5, _Power);
            }
            
			// vertex function that "builds the object"; takes an iterated vertex point and adjusts its position, returning in a form that will be passed to the fragment function 
            v2f vert(appdata v) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.uv = v.uv;
                return OUT;
            }
			
			// fragment function that "colors the object"; takes an iterated pixel and gets its color
            fixed4 frag(v2f IN) : SV_TARGET {  
                float2 uv = IN.uv.xy;
                float c = PerlinNoise2D(uv);

				return fixed4(c, c, c, c) * _Color;
            }
 
            ENDCG
 
        }
    }
}