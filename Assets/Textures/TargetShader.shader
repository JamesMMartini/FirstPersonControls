// This shader visuzlizes the normal vector values on the mesh.
Shader "Custom/TargetShader"
{
    Properties
    {
        _Color1("Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

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

            float4 _Color1;

            float nrand(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                //OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
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

                return _Color1;
            }
            ENDHLSL
        }
    }
}
