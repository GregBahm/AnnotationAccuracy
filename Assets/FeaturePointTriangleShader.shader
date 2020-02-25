Shader "Unlit/FeaturePointTriangleShader"
{
    Properties
    {
		_RingFrequency("Ring Frequence", Float) = 1
		_RingThickness("Ring Thickness", Range(0, 1)) = .5
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
		Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float3 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float3 color : COLOR;
				float3 worldPos : TEXCOORD1;
            };

			float3 _CursorPos;
			float4 _Color;

			float _RingFrequency;
			float _RingThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = mul(unity_ObjectToWorld, v.normal);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

			float3 GetLightningColor(float3 normal)
			{
				float shade = dot(normal, float3(0, 1, 0)) * .5 + .5;

				float3 lightingColor = lerp(float3(0, 1, 1), float3(1, 0, 1), shade);
				lightingColor = pow(lightingColor, 2);
				return lightingColor;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float3 edgeDist = 1 - i.color;
				float alpha = max(edgeDist.x, max(edgeDist.y, edgeDist.z));
				alpha = pow(alpha, 100) * .5;

				float dist = 1 - length(i.worldPos - _CursorPos) * 10;
				float distAlpha = dist * .5;

				float waves = (dist % _RingFrequency) / _RingFrequency;
				waves = 1 - abs(waves - .5) * 2;
				waves = (waves - _RingThickness) * 20;
				waves = saturate(waves);

				float3 norm = normalize(i.normal);
				float3 lightingColor = GetLightningColor(norm);

				lightingColor = lerp(lightingColor, 1, alpha * 10);
				alpha = alpha + distAlpha;
				return float4(lightingColor, alpha);
            }
            ENDCG
        }
    }
}
