Shader "Unlit/Waves"
{
    
    Properties
    {
        // Color property for material inspector, default to white
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            //param
            #pragma shader_feature SINE
            #pragma shader_feature SHARP_SINE

            struct Wave
            {
                float1 amplitude;
                float1 wavelength;
                float2 direction;
            };
            int numberOfWaves;
            StructuredBuffer<Wave> waves;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 norm : TEXCOORD0;
            };

            float SineWaveHeight(Wave w, float2 pos)
            {
                return w.amplitude * sin(dot(w.direction, pos) * w.wavelength + _Time.y);
            }

            float3 SineWaveNorm(Wave w, float2 pos)
            {
                return float3(
                    float2(w.wavelength * w.direction.xy * w.amplitude * cos(dot(w.direction, pos) * w.wavelength + _Time.y)),
                    0
                );
            }

            float SharpSineWaveHeight(Wave w, float2 pos)
            {
                return 2.0f * w.amplitude * pow(
                    (sin(dot(w.direction, pos) * w.wavelength + _Time.y) + 1.0f) / 2.0f, 
                    3.0f
                );
            }

            float3 SharpSineWaveNorm(Wave w, float2 pos)
            {
                return float3(
                    float2(
                        3.0f * w.wavelength * w.direction.xy * w.amplitude * 
                        pow(((sin(dot(w.direction, pos) * w.wavelength + _Time.y) + 1.0f) / 2.0f), 2.0f) * 
                        cos(dot(w.direction, pos) * w.wavelength + _Time.y)
                        ),
                    0
                );
            }

            v2f vert (float4 vertex : POSITION)
            {
                v2f ret;

                // height of the wave
                float1 totalHeight = 0.f;
                float3 norm = float3(0, 0, 1);
                for(int i = 0; i < numberOfWaves; i++)
                {
                    #ifdef SINE
                        totalHeight += SineWaveHeight(waves[i], vertex.xz),
                        norm += SineWaveNorm(waves[i], vertex.xz);
                    #endif
                    #ifdef SHARP_SINE
                        totalHeight += SharpSineWaveHeight(waves[i], vertex.xz),
                        norm += SharpSineWaveNorm(waves[i], vertex.xz);
                    #endif
                }

                vertex += float4(0, totalHeight, 0, 0);

                ret.pos = UnityObjectToClipPos(vertex);
                ret.norm = norm;

                return ret;
            }
            
            // color from the material
            fixed4 _Color;
            float3 sun_dir; 

            // pixel shader, no inputs needed
            fixed4 frag (v2f data) : SV_Target
            {
                float1 cos_theta = dot(sun_dir, data.norm);
                return half4(_Color.xyz * cos_theta, 1);
            }
            ENDCG
        }
    }
}