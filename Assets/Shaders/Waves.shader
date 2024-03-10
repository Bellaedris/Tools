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

            struct Wave
            {
                float1 amplitude;
                float1 wavelength;
                float2 direction;
            };
            int numberOfWaves;
            StructuredBuffer<Wave> waves;

            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                // height of the wave
                float1 totalHeight = 0.f;
                for(int i = 0; i < numberOfWaves; i++)
                {
                    //totalHeight += waves[i].amplitude * sin(dot(waves[i].direction, vertex.xy * waves[i].wavelength)  + _Time.y * 1);
                    totalHeight += waves[i].amplitude * sin(dot(waves[i].direction, vertex.xz) * waves[i].wavelength + _Time.y);
                }
                vertex += float4(0, totalHeight, 0, 0);

                return UnityObjectToClipPos(vertex);
            }
            
            // color from the material
            fixed4 _Color;

            // pixel shader, no inputs needed
            fixed4 frag () : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}