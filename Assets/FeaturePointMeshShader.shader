Shader "Unlit/FeaturePointMeshShader"
{
    Properties
    {
		_FogStart("Fog Start", Float) = 1
		_FogEnd("Fog End", Float) = 1
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" }
        LOD 100

		Cull Off
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
			float _FogStart;
			float _FogEnd;

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
				float depth = i.vertex.z;
				float fog = (depth - _FogStart) / (_FogEnd - _FogStart);

				//return float4(i.color, 1);
				//return float4(i.color, 1);
				float3 edgeDist = 1 - i.color;
				float edgeAlpha = max(edgeDist.x, max(edgeDist.y, edgeDist.z));
				edgeAlpha = pow(edgeAlpha, 100) * .4;

				float dist = 1 - length(i.worldPos - _CursorPos) * 20;
				float distAlpha = dist;
				//return float4(1, 1, 1, distAlpha);


				return float4(1, 1, 1, distAlpha + edgeAlpha);

				//alpha = alpha + saturate(distAlpha);
				//return float4(1, 1, 1, alpha);
            }
            ENDCG
        }
    }
}
