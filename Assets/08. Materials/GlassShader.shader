Shader "Custom/GlassShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,0.5)
        _Smoothness ("Smoothness", Range(0,1)) = 0.9
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            half4 _BaseColor;
            float _Smoothness;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.worldPos = TransformObjectToWorld(input.positionOS).xyz;
                output.worldNormal = TransformObjectToWorldNormal(input.positionOS.xyz);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                half3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);
                half3 worldNormal = normalize(input.worldNormal);
                half3 reflection = reflect(-viewDir, worldNormal);
                half4 reflectionColor = SAMPLE_TEXTURECUBE(_EnvCubemap, reflection);

                half4 color = lerp(_BaseColor, reflectionColor, _Smoothness);
                color.a = _BaseColor.a;
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
