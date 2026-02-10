Shader "Custom/ParticleInvertCircle"
{
    Properties
    {
        // 파티클 시스템에서 보내주는 텍스처(보통 흰색)를 받기 위함이야
        [HideInInspector] _MainTex ("Particle Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        
        // 배경색을 반전시키는 마법의 블렌딩!
        Blend OneMinusDstColor OneMinusSrcColor
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // 파티클 시스템의 컬러 값을 받아와
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 1. UV의 중심(0.5, 0.5)에서 현재 픽셀까지의 거리를 계산해
                float dist = distance(i.uv, float2(0.5, 0.5));
                
                // 2. 거리가 0.5보다 작으면 1(흰색), 크면 0(검은색)으로 만들어서 원을 그려
                // smoothstep을 살짝 써서 테두리를 부드럽게 깎아주자고~
                float circle = smoothstep(0.5, 0.48, dist);
                
                // 3. 파티클의 Alpha 값과 계산된 원을 곱해줘
                // 이렇게 해야 파티클 시스템의 'Size over Lifetime'이나 'Fade'가 먹히거든
                float finalAlpha = circle * i.color.a;
                
                // 결과값이 흰색일수록 배경이 더 잘 반전돼!
                return fixed4(finalAlpha, finalAlpha, finalAlpha, finalAlpha);
            }
            ENDCG
        }
    }
}