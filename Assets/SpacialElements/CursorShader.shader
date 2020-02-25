Shader "Unlit/CursorShader"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
		_Opacity("Opacity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
            };

			float4 _Color;
			float4 _ShadowColor;
			float _Opacity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.normal = mul(unity_ObjectToWorld, v.normal);
                return o;
            }

			float3 GetLightningColor(float3 normal)
			{
				float shade = dot(normal, float3(0, 1, 0)) * .5 + .5;

				float3 lightingColor = lerp(float3(0, 1, 1), float3(1, 0, 1), shade);
				lightingColor = pow(lightingColor, 4);
				lightingColor = lerp(lightingColor, shade * 3, .8);
				return lightingColor;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float3 norm = normalize(i.normal);
				float3 lightingColor = GetLightningColor(norm);
				float3 ret = lightingColor * _Color;
				return float4(ret, _Opacity);

            }
            ENDCG
        }
    }
}
