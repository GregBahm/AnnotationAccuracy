Shader "Unlit/DottedLineShader"
{
    Properties
    {
		_Frequency("Frequency", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

			float _Frequency;
			float _LineLength;
			float _Opacity;

            fixed4 frag (v2f i) : SV_Target
            {
				float val = i.uv.x * _LineLength * 100;
				float dotted = val % _Frequency;
				dotted = abs(dotted - .5) * 2;
				dotted = saturate(dotted - .5) * 20;
				return dotted;
            }
            ENDCG
        }
    }
}
