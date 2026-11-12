Shader "Custom/FaceMosaic"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _BlockSize ("Mosaic Block Size (Pixels)", Float) = 24.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // x=1/w, y=1/h, z=w, w=h
            float _BlockSize;

            // Up to 20 simultaneous faces: (xMin, yMin, xMax, yMax) in normalized 0..1 UVs
            int _FaceCount;
            float4 _FaceBoxes[20];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                bool inFace = false;

                for (int idx = 0; idx < _FaceCount; idx++)
                {
                    float4 box = _FaceBoxes[idx];
                    if (uv.x >= box.x && uv.x <= box.z && uv.y >= box.y && uv.y <= box.w)
                    {
                        inFace = true;
                        break;
                    }
                }

                if (inFace)
                {
                    // Snap UVs to a grid based on texture resolution and _BlockSize
                    float2 texSize = _MainTex_TexelSize.zw;
                    float2 blocks = max(float2(1.0, 1.0), texSize / _BlockSize);
                    uv = (floor(uv * blocks) + 0.5) / blocks;
                }

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
