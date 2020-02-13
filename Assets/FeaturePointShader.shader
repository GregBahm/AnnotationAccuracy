Shader "Unlit/FeaturePointShader"
{
    Properties
    {
		//_PulseWaveSize("PulseWaveSize", Float) = 1
		//_PulseProgression("Pulse Progression", Range(-1, 2)) = 0
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" }
        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };  

			float3 _PulseCenter;
			float _PulseWaveSize;
			float _PulseProgression;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, float4(0,0,0, 1));
                return o;
            }

			float GetPulseProgress(float3 worldPos)
			{
				float distToPulseCenter = length(worldPos - _PulseCenter);
				float waveDistance = _PulseProgression * _PulseWaveSize;
				float distToWave = abs(distToPulseCenter - waveDistance) / _PulseWaveSize;
				return saturate(1 - distToWave);
			}

            fixed4 frag (v2f i) : SV_Target
            { 
				float pulse = GetPulseProgress(i.worldPos);
				float maxRadius = lerp(-.1, .4, pulse);
				float distTocenter = length(i.uv - .5);
				float alpha = 1 - (distTocenter - maxRadius) * 100;
				alpha = saturate(alpha);
				float3 col = float3(1, 1, 1);//TODO: Make this cooler
				return float4(col, alpha);
            }
            ENDCG
        }
    }
}
