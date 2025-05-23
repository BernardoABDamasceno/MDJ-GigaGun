Shader "Custom/PS1_Graphics"
{
    Properties
    {
        [Header(PS1 Texture Options)]
        _MainTexture("Main Texture", 2D) = "white" {}
        _AnimateXY("Animate X Y", Vector) = (0, 0, 0, 0)
        _DitherGamma("Dither Gamma", Range(0.1, 2)) = 1.0

        [Header(PS1 Light Options)]
        _COLOR("Color", COLOR) = (1,1,1,1)
        _AmbientLight("Ambient Light", COLOR) = (0,0,0)
        _LightIntensity("Light Intensity", float) = 1.0
        _LightDirection("Light Direction", Vector) = (0,1,0)

        [Header(PS1 Graphics Style Options)]
        [Toggle] _VertexSnapping("Vertex Snapping", float) = 0.0
        [Toggle] _AffineTextureMapping("Affine Texture Mapping", float) = 0.0
        [Toggle] _DynamicVertexLighting("Dynamic Vertex Lighting", float) = 0.0
        [Toggle] _Dithering("Dithering", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex vertexShader
            #pragma fragment fragmentShader

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {

                float4 position : SV_POSITION;

                // noperspective keyword does the ps1 effect since it didn't do 3d, so important
                // to add toggle the keyword must be removed and the w divison must be added again
                float2 uv : TEXCOORD0;

                float3 normal : TEXCOORD1;

                float3 color : TEXCOORD2;

                float4 screenCoord : TEXCOORD3;

                // it seems to apply this correctly, noperspective is not needed, 
                // since the later division gives the same effect as if the other two(uv and color) had the noperspective keyword
                float4 clipW : TEXCOORD4;
            };

            sampler2D _MainTexture;
            float4 _MainTexture_ST;
            float4 _AnimateXY;
            float _DitherGamma;

            float3 _LightDirection;
            float3 _AmbientLight;
            float _LightIntensity;
            float4 _COLOR;
            
            // Toggles
            float _VertexSnapping;
            float _DynamicVertexLighting;
            float _Dithering;
            float _AffineTextureMapping;
            
            v2f vertexShader(VertexData v) {
                v2f i;
                
                //snap to nearest pixel
                float4 clip = UnityObjectToClipPos(v.position);
                float4 vertex = clip;

                if(_VertexSnapping != 0.0){
                    vertex.xy = round(clip.xy / clip.w * _ScreenParams.xy) / _ScreenParams.xy * clip.w;
                }

                i.position = vertex;
                
                // now i need to save clip.w for later
                i.clipW = clip.w;
                
                //texture
                i.uv = TRANSFORM_TEX(v.uv, _MainTexture);
                i.uv += frac(_AnimateXY.xy * _MainTexture_ST.xy * _Time.yy); // Animate UVs based on time

                i.normal = v.normal;

                //dynamic vertex lighting
                if(_DynamicVertexLighting != 0.0){
                    float direct_light = clamp(dot(UnityObjectToWorldNormal(v.normal), normalize(_LightDirection)), 0.0, 1.0);
                    float3 light = _AmbientLight + direct_light * _LightIntensity;
                    _COLOR.rgb *= clamp(light, float3(0,0,0), float3(1,1,1));
                }

                //save for later
                i.color = _COLOR.rgb;
                i.screenCoord = ComputeScreenPos(i.position);
                
                if(_AffineTextureMapping != 0.0){
                    i.uv *= i.clipW.w;
                    i.color *= i.clipW.w;
                }

                return i;
            }

            fixed4 fragmentShader(v2f i) : SV_TARGET {
        
                float2 uvs = i.uv;
                float3 color = i.color;

                if(_AffineTextureMapping){
                    uvs /= i.clipW.w;
                    color /= i.clipW.w;
                }
                
                fixed4 textureColor = tex2D(_MainTexture, uvs);
                
                float3 quantized = textureColor.rgb * color;
                
                if (_Dithering != 0.0 ){
                    
                    float2 fragCoord = i.screenCoord.xy / i.screenCoord.w;
                    
                    int ps1_dither_matrix[16] = {
                        -4,  0, -3,  1,
                        2, -2,  3, -1,
                        -3,  1, -4,  0,
                        3, -1,  2, -2
                    };

                    float noise = float(ps1_dither_matrix[(int(fragCoord.x) % 4) + (int(fragCoord.y) % 4) * 4]);

                    quantized = pow(quantized, float3(1.0 / _DitherGamma,1.0 / _DitherGamma,1.0 / _DitherGamma));
                    quantized = round(quantized * 255.0 + noise);
                    quantized = clamp(round(quantized), float3(0.0, 0.0, 0.0), float3(255, 255, 255));
                    quantized = clamp(quantized / 8, float3(0.0, 0.0, 0.0), float3(31, 31, 31));
                    quantized /= 31.0;

                    quantized = pow(quantized, float3(_DitherGamma, _DitherGamma, _DitherGamma));
                }

                return fixed4(quantized, 1.0);
            }
            // to make a fix to the distortion of affine texture mapping, a tecelation layer is needed
            // however keeping at least a bit of it would look better, could just be used to maybe have lower poly models

            ENDCG
        }
    }
    FallBack "Diffuse"
}
