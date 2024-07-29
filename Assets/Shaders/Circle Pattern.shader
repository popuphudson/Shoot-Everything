Shader "Unlit/Circle Pattern"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Freq ("Frequency", Float) = 4.0
        _LineThickness ("Line Thickness", Float) = .01
        _Size ("Size", Float) = 1.0
        _Seed ("Seed", Float) = 302.55
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float _Freq;
            float _LineThickness;
            float _Size;
            float _Seed;

            float Hash21(float2 p) {
                return frac(dot(sin(p*230.312), p*591.292)*_Seed);
            }

            float3 CirclePattern(float2 uv) {
                float3 col = 0;
                float2 gv = frac(uv)-.5;
                float2 id = floor(uv);
                
                for(int x=-1;x<=1;x++) {
                    for(int y=-1;y<=1;y++) {
                        float2 offset = float2(x, y);
                        float d = length(gv-offset);
                        float2 offsetid = id+offset;
                        float rand = Hash21(offsetid-float2(3., 3.)*floor(offsetid/float2(3., 3.)));
                        
                        d = sin(d*_Freq-(_Time.y/2.))/_Freq;
                        d = step(1./(_Freq+_LineThickness), d);
                        col += d;
                        
                        if(d > 0.) col *= float3((rand/2.)+.5, frac(rand*454.222), frac(rand*932.07));
                    }
                }
                
                return col;
            }

            float3 BackgroundPattern(float2 uv) {
                float3 col = 0;
                
                col += float3(sin(_Time.y-_Seed), cos(_Time.y-_Seed), sin(_Seed-_Time.y));
                
                return col;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                uv *= _Size;
                float3 col = BackgroundPattern(uv);
                col += CirclePattern(uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return fixed4(col,1.);
            }
            ENDCG
        }
    }
}
