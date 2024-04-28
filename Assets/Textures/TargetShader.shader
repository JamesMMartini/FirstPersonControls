Shader "Custom/TargetShader"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    {
        _Color1("Color", Color) = (0, 0, 0, 1)

        //_BaseMap("Base Map", 2D) = "white"
        //_Offset("Offset", Float) = 0.0
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            float nrand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float4 _Color1;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = float2(IN.uv.x, IN.uv.y);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float xDistToEdge = (IN.uv.x < 0.5) ? IN.uv.x : 1 - IN.uv.x;
                float yDistToEdge = (IN.uv.y < 0.5) ? IN.uv.y : 1 - IN.uv.y;

                if (xDistToEdge > 0.05 && yDistToEdge > 0.05)
                {
                    xDistToEdge += (nrand(IN.uv) * nrand(IN.uv));
                    yDistToEdge += (nrand(IN.uv) * nrand(IN.uv));

                    if (xDistToEdge > 0.4 && yDistToEdge > 0.4)
                        clip(-1);
                }

                // The SAMPLE_TEXTURE2D marco samples the texture with the given
                // sampler.

                //IN.uv.y = IN.uv.y + offset;
                //IN.uv.y = IN.uv.y + ((_Time[1] / 5) + (nrand(IN.uv) / 30));

                //half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                return _Color1;
            }
            ENDHLSL
        }
    }
}
