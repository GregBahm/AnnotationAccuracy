Shader "Unlit/FancyTargettingLineSegmentShader"
{
    Properties
    {
		_SegmentLength("Seg Length", Range(0, 1)) = .5
		_RingSize("Ring Size", Range(0, 1)) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Blend One One
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			float _SegmentLength;
			float _RingSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float param = _RingSize / _SegmentLength;
				if (i.uv.x < param)
				{
					return 0;
				}
				float ret = (1 - i.uv.x) * _SegmentLength;
				ret = ret / (_SegmentLength - _RingSize);
				ret = pow(ret, 1);
				return ret;
            }
            ENDCG
        }
    }
}
