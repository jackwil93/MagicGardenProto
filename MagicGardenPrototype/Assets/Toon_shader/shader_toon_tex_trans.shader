// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/shader_toon_texture_trans" {
	  Properties {    
		      _Color ("Outline Color", Color) = (0,0,0,1)
		      _SpecColor ("Specular Color", Color) = (1,1,1,1) 
		      _Shininess ("Shininess", Float) = 10
		      _Transparenc1 ("transparency 1", Range(0,1)) = 0.2
		      _Transparenc2 ("transparency 2", Range(0,1)) = 0.2
		   } 
	 
	 SubShader {
      Tags { "Queue" = "Transparent" } 
       
       Pass {
         Cull Front
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha 

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         #include "UnityCG.cginc"
         #include "AutoLight.cginc"
         #include "Lighting.cginc"
         #pragma multi_compile_fwdbase
         
         
         uniform float4 _Color;
         uniform float _Shininess;
         uniform float _Transparenc1;
         
         
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
            LIGHTING_COORDS(3,4)
         };
 
         vertexOutput vert(vertexInput input)
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).rgb);
            output.pos = UnityObjectToClipPos(input.vertex);
     
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            
            float3 normalDirection = normalize(input.normalDir);
 			//float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb;
            float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.rgb);
            float3 lightDirection;
            float attenuation;
            
            if (0.0 == _WorldSpaceLightPos0.w) 
            {
               attenuation = 1.0; 
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else 
            {
               float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; 
               lightDirection = normalize(vertexToLightSource);
            }
            
            float3 fragmentColor = _Color;
            if (dot(normalDirection, lightDirection) > 0.0 
               && attenuation *  pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)), _Shininess) < 0.5) 
               
            {
               fragmentColor = _SpecColor.a * _LightColor0.rgb * _SpecColor.rgb + (1.0 - _SpecColor.a) * fragmentColor;
            }
            
            return float4(fragmentColor, _Transparenc1);
         }
 
         ENDCG  
      }

      Pass {
         Cull Back 
         ZWrite Off 
         Blend SrcAlpha OneMinusSrcAlpha 
        
         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         #include "UnityCG.cginc"
         #include "AutoLight.cginc"
         #include "Lighting.cginc"
         #pragma multi_compile_fwdbase
         
         uniform float4 _Color;
         uniform float _Shininess;
         uniform float _Transparenc2;
         
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
            LIGHTING_COORDS(3,4)
         };
 
         vertexOutput vert(vertexInput input)
         {
             vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
 		
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).rgb);
            output.pos = UnityObjectToClipPos(input.vertex);
            
            TRANSFER_VERTEX_TO_FRAGMENT(output);
            
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
 			//float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb;
 			
            float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.rgb);
            float3 lightDirection;
            float attenuation;
            
            if (0.0 == _WorldSpaceLightPos0.w) 
            {
               attenuation = 1.0;
               lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            } 
            else 
            {
               float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance;  
               lightDirection = normalize(vertexToLightSource);
            }
            
            float3 fragmentColor = _Color;
            if (dot(normalDirection, lightDirection) > 0.0 
               && attenuation *  pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)), _Shininess) > 0.5) 
            {
               fragmentColor = _SpecColor.a * _LightColor0.rgb * _SpecColor.rgb + (1.0 - _SpecColor.a) * fragmentColor;
            }
            
            return float4(fragmentColor, _Transparenc2);
         }
 
         ENDCG  
      }
   }
}
