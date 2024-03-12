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
                float3 worldPos : TEXCOORD1;
            };

            float SineWaveHeight(Wave w, float2 pos, float gain, float loss)
            {
                float amp = w.amplitude * loss;
                float wl = w.wavelength * gain;
                return amp * sin(dot(w.direction, pos) * wl + _Time.y);
            }

            float3 SineWaveNorm(Wave w, float2 pos, float gain, float loss)
            {
                float amp = w.amplitude * loss;
                float wl = w.wavelength * gain;
                return float3(
                    float2(wl * w.direction.xy * amp * cos(dot(w.direction, pos) * wl + _Time.y)),
                    0
                );
            }

            float SharpSineWaveHeight(Wave w, float2 pos, float gain, float loss)
            {
                float amp = w.amplitude * loss;
                float wl = w.wavelength * gain;
                return 2.0f * amp * pow(
                    (sin(dot(w.direction, pos) * wl + _Time.y) + 1.0f) / 2.0f, 
                    3.0f
                );
            }

            float3 SharpSineWaveNorm(Wave w, float2 pos, float gain, float loss)
            {
                float amp = w.amplitude * loss;
                float wl = w.wavelength * gain;
                return float3(
                    float2(
                        3.0f * wl * w.direction.xy * amp * 
                        pow(((sin(dot(w.direction, pos) * wl + _Time.y) + 1.0f) / 2.0f), 2.0f) * 
                        cos(dot(w.direction, pos) * wl + _Time.y)
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
                float gain = 1;
                float loss = 1;
                for(int i = 0; i < numberOfWaves; i++)
                {
                    #ifdef SINE
                        totalHeight += SineWaveHeight(waves[i], vertex.xz, gain, loss),
                        norm += SineWaveNorm(waves[i], vertex.xz, gain, loss);
                    #endif
                    #ifdef SHARP_SINE
                        totalHeight += SharpSineWaveHeight(waves[i], vertex.xz, gain, loss),
                        norm += SharpSineWaveNorm(waves[i], vertex.xz, gain, loss);
                    #endif
                    gain *= 1.25;
                    loss *= 0.75;
                }

                vertex -= float4(0, totalHeight, 0, 0);

                ret.pos = UnityObjectToClipPos(vertex);
                ret.worldPos = mul(unity_ObjectToWorld, vertex);
                ret.norm = norm;

                return ret;
            }
            
            // color from the material
            fixed4 _Color;
            float3 sun_dir; 
            float4 sunColor;
            float3 cameraPos;
            float specularStrength;

            // pixel shader, no inputs needed
            fixed4 frag (v2f data) : SV_Target
            {
                float3 norm = normalize(data.norm);
                float3 light_dir = normalize(sun_dir);

                float1 cos_theta = dot(light_dir, norm);
                float3 diffuse = sunColor.xyz * cos_theta;

                float3 viewDir = normalize(_WorldSpaceCameraPos - data.worldPos.xyz);
                float3 reflectDir = reflect(light_dir, norm);
                float3 spec = specularStrength * sunColor.xyz * pow(max(dot(viewDir, reflectDir), 0.0), 32);

                return half4((diffuse + spec) * _Color, 1);
            }
            ENDCG
        }
    }
}