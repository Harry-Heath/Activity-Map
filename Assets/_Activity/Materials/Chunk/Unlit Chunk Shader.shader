Shader "Chunk Shader"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        HLSLINCLUDE

        #pragma multi_compile_instancing

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

        struct Attributes
        {
            float4 pos : POSITION;
            float2 uv  : TEXCOORD0;
        };

        struct Varyings
        {
            float4 pos : SV_POSITION;
            float4 colour : COLOR;
        };

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.colour = SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, IN.uv, 0);

            IN.pos.y += saturate(1 - length(OUT.colour));

            OUT.pos = TransformObjectToHClip(IN.pos.xyz);
            return OUT;
        }

        float4 frag(Varyings IN) : SV_Target
        {
            return IN.colour;
        }

        ENDHLSL

        Pass
        {
            Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
			Tags { "LightMode"="DepthNormalsOnly" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}