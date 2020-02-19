Shader "Unlit/FeaturePointMeshShader"
{
    Properties
    {
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
        
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

            v2f vert (appdata v)
            {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal * .5 + .5;
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//return float4(i.color, 1);
				float3 edgeDist = 1 - i.color;
				float alpha = min(edgeDist.x, max(edgeDist.y, edgeDist.z));
				alpha = pow(alpha, 10);

				float dist = 1 - length(i.worldPos - _CursorPos) * 5;
				float distAlpha = dist;
				//return float4(1, 1, 1, distAlpha);

				alpha = alpha + saturate(distAlpha);
				return float4(1, 1, 1, alpha);
            }
            ENDCG
        }
    }
}
