Shader "Instanced Colour Shader"
{
	Properties
	{ }

	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

		HLSLINCLUDE

		#pragma multi_compile_instancing

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

		struct Attributes
		{
			float4 positionOS : POSITION;
			uint instanceID : SV_InstanceID;
		};

		struct Varyings
		{
			float4 positionHCS  : SV_POSITION;
			float4 colour : COLOR;
		};

		StructuredBuffer<float2> _Colours;

		Varyings vert(Attributes IN)
		{
			UNITY_SETUP_INSTANCE_ID(IN);

			Varyings OUT;
			OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
			OUT.colour = float4(_Colours[IN.instanceID], 0, 1);
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