Shader "Li RP/Unlit_Lit"
{
    Properties
    {
        _BaseMap("Texture",2D) = "white"{}
        _BaseColor("Color", Color) = (1.0,1.0,1.0,1.0)
//        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
//        [Toggle(_CLIPPING)] _Clipping("Alpha Clipping", Float) = 0
//        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Scr Blend",Float) = 1
//        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend",Float) = 0
//        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
    }
    SubShader
    {

        Pass
        {
            Tags{
                "LightMode" = "CustomLit"
            }
//            Blend [_SrcBlend] [_DstBlend]
//            ZWrite [_ZWrite]
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _CLIPPING
            #pragma multi_compile_instancing
            #include "./ShaderLibrary/Common.hlsl"

            // CBUFFER_START(UnityPerMaterial)
            //     float4 _BaseColor;
            // CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            // INSTANCING 和 batch
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
            //float4 _BaseColor;
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseMap_ST)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                // UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)


            struct Attributes
            {
                float3 positionOS:POSITION;
                float3 normalOS:NORMAL;
                float2 baseUV:TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS:SV_POSITION;
                float3 normalWS:VAR_NORMAL;
                float2 baseUV :VAR_BASE_UV;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            //float4 vert(float3 positionOS:POSITION):SV_POSITION{
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                float3 positionWS = TransformObjectToWorld(input.positionOS);

                output.positionCS = TransformWorldToHClip(positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseMap_ST);
                output.baseUV = input.baseUV * baseST.xy + baseST.zw;
                return output;
            }

            float4 frag(Varyings input):SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.baseUV);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
                float4 base = baseMap * baseColor;
                // base.rgb = abs(length(input.normalWS)-1.0) *10.0;
                base.rgb = normalize(input.normalWS);
                // #if defined(_CLIPPING)
                //     clip(base.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
                // #endif

                return base;
            }
            ENDHLSL
        }
    }
}