Shader "Custom/Cutout"
{
    Properties
    {
        _MainTex("Texture", 2D) = "" {}
        _Tint("Tint", Color) = (1, 1, 1, 1)
        _CutoutTex("Cutout Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _CutoutTex;
            float4 _CutoutTex_ST;
            float4 _Tint;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f input) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, input.uv) * _Tint;
                fixed4 cutout = tex2D(_CutoutTex, input.uv);
                color.a -= cutout.a;
                return color;
            }
            ENDCG
        }
    }
}
