Shader "Unlit/FeaturePointTriangleShader"
{
    Properties
    {
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
            };

			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal * .5 + .5;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return float4(i.normal, .5);
            }
            ENDCG
        }
    }
}
