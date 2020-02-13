Shader "Unlit/CursorDistanceShader"
{
    Properties
    {
		_RingFrequency("Ring Frequence", Float) = 1
		_RingThickness("Ring Thickness", Range(0, 1)) = .5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Pass
        {

			ZWrite Off
			Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float3 centerPos : TEXCOORD2;
            };

			float _RingFrequency;
			float _RingThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.centerPos = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float absoluteCenterDist = length(i.worldPos - i.centerPos);
				float relativeCenterDist = length(i.uv - .5) * 2;
				float secondAlpha = (1 - pow(relativeCenterDist, 10)) * pow(relativeCenterDist, 10);
				//return secondAlpha;
				float waves = (absoluteCenterDist % _RingFrequency) / _RingFrequency;
				waves = 1 - abs(waves - .5) * 2;
				waves = (waves - _RingThickness) * 20;
				waves = saturate(waves) * saturate(secondAlpha);
				return waves;
            }
            ENDCG
        }
    }
}
