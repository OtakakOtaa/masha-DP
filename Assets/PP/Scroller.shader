Shader "Custom/2DScrollingCloudsTransparent"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _ScrollSpeedX("Scroll Speed X", Range(-1, 1)) = 0.5
        _ScrollSpeedY("Scroll Speed Y", Range(-1, 1)) = 0.0
        _TransparentColor("Transparent Color", Color) = (1, 0, 0, 1)
        [Enum(X,0,Y,1)] _ScrollAxis("Scroll Axis", Int) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            int _ScrollAxis;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 _TransparentColor;

            fixed4 frag(v2f i) : SV_Target
            {
                // Scroll the texture based on the selected axis
                if (_ScrollAxis == 0)
                {
                    i.uv.x = fmod(i.uv.x + _Time.x * _ScrollSpeedX, 1.0);
                }
                else
                {
                    i.uv.y = fmod(i.uv.y + _Time.x * _ScrollSpeedY, 1.0);
                }

                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate the alpha value based on the difference between the pixel color and the transparent color
                fixed alpha = 1.0 - distance(col.rgb, _TransparentColor.rgb);

                // Apply the calculated alpha value to the final color
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}