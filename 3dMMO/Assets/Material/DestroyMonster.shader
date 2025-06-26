Shader "Unlit/DestroyMonster"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NoiseTex("Noise (RGB)", 2D) = "white" {}
        _Cut("Alpha Cut", Range(0,1)) = 0
        [HDR]_OutColor("OutColor", Color) = (1,1,1,1)
        _OutThinkness("OutThinkness", Range(1,1.5)) = 1.15
        _LightDir("Light Direction", Vector) = (0,1,0,0)
        _LightIntensity("Light Intensity", Float) = 1.0
    }
    SubShader
    {
          
        Tags { "RenderType"="Opaque"}
        LOD 100

        Pass
        {
             Name "Forward"
            Tags { "LightMode" = "UniversalForward" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                    float3 worldNormal : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Cut;
            float4 _OutColor;
            float _OutThinkness;
            float4 _LightDir;
            float _LightIntensity;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
               fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 noise = tex2D(_NoiseTex, i.uv);
                clip(noise.r - _Cut);

                float edge = step(_Cut, noise.r) - step(_Cut * _OutThinkness, noise.r);

                fixed4 finalColor = lerp(texColor, _OutColor, edge);


                float3 lightDir = normalize(_LightDir.xyz); // 스크립트에서 전달받음
                float NdotL = saturate(dot(normalize(i.worldNormal), normalize(_LightDir.xyz)));
                finalColor.rgb *= 0.5 + 0.5 * NdotL;

                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                return finalColor;
            }
            ENDCG
        }
            Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ColorMask 0
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
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

            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Cut;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noiseVal = tex2D(_NoiseTex, i.uv).r;
                clip(noiseVal - _Cut); 
                return 0;
            }
            ENDCG
        }
    }
}
