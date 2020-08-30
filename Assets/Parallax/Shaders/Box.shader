Shader "Custom/Box"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Col ("Col", Range(0,20)) = 1
        _Row ("Row", Range(0,20)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float grid(float2 st, float size)
            {
                size = 0.5 + size * 0.5;
                st = step(st, size) * step(1.0 - st, size);
                return st.x * st.y;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Col;
            float _Row;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st = frac(i.uv * float2(_Col, _Row));
                return 1 - grid(st, 0.98);
            }
            ENDCG
        }
    }
}
