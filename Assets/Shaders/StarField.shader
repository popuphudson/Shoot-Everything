Shader "Unlit/StarField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #define NUM_LAYERS 6.

            float2x2 Rot(float a) {
                float s = sin(a);
                float c = cos(a);
                return float2x2(c, -s, s, c);
            }

            float Star(float2 uv, float flare) {
                float d = length(uv);
                float m = .03/d;
                
                float rays = max(0., 1.-abs(uv.x*uv.y*5000.));
                m += rays*flare;
                
                uv = mul(uv, Rot(3.14/4.));
                rays = max(0., 1.-abs(uv.x*uv.y*5000.));
                m += rays*flare;
                m *= smoothstep(1., .2, d);
                return m;
            }

            float Hash21(float2 p) {
                p = frac(p*float2(123.34, 456.21));
                p += dot(p, p+45.32);
                return frac(p.x*p.y);
            }

            float3 StarLayer(float2 uv, float depth) {
                float3 col = float3(0,0,0);
                
                float2 gv = frac(uv)-.5;
                float2 id = floor(uv);
                
                for(int y=-1;y<=1;y++) {
                    for(int x=-1;x<=1;x++) {
                        
                        float2 offset = float2(x, y);
                        
                        float n = Hash21(id+offset);
                
                        float size = frac(n*345.32);
                
                        float star = Star(gv-offset-float2(n,frac(n*34.))+.5, smoothstep(.9, 1., size)*.3*depth);
                            
                        float3 colour = sin(float3(.2, .3, .9)*frac(n*2345.2)*123.2)*.5+.5;    
                        colour = colour*float3(.8, .5, 1.+size);
                        
                        col += star*size*colour;
                    }  
                }
                return col;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float t = _Time.y*.1;
                
                float3 col = float3(0,0,0);
                
                for(float i=0.; i<1.; i+=1./NUM_LAYERS) {
                    float depth = frac(i+t);
                    float scale = lerp(50., .75, depth);
                    float fade = depth*smoothstep(1., .9, depth);
                    uv = mul(uv, Rot(t*.3*i));
                    col += StarLayer(uv*scale+(i*453.2), depth)*fade;
                }
                
                return fixed4(col.x, col.y, col.z, 1.0);
            }
            ENDCG
        }
    }
}
