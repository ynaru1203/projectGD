Shader "UI/InvertCircle"
{
    Properties
    {
        _Thickness ("Thickness", Range(0.01, 0.5)) = 0.05
        _Radius ("Radius", Range(0.0, 0.5)) = 0.4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // 배경을 반전시키는 블렌딩 설정
        Blend OneMinusDstColor OneMinusSrcColor
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            float _Thickness;
            float _Radius;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // UV 좌표의 중심(0.5, 0.5)으로부터의 거리를 계산해 (원을 그리는 공식이야)
                float dist = distance(i.uv, float2(0.5, 0.5));
                
                // 원의 테두리 부분만 1(흰색)로 만들고 나머지는 0(검은색)으로 만들어
                // 흰색인 부분만 배경을 반전시키게 될 거야!
                float circle = smoothstep(_Radius, _Radius - 0.01, dist) - 
                               smoothstep(_Radius - _Thickness, _Radius - _Thickness - 0.01, dist);
                
                return fixed4(circle, circle, circle, circle);
            }
            ENDCG
        }
    }
}