// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/shader_toon_texture" {
	 
	 Properties {
      _MainTex ("Texture Image", 2D) = "white" {}
      _DiffuseThreshold ("Threshold for Diffuse Colors", Range(0,1))= 0.1 
      _OutlineColor ("Outline Color", Color) = (0,0,0,1)
      _LitOutlineThickness ("Lit Outline Thickness", Range(0,1)) = 0.1
      _UnlitOutlineThickness ("Unlit Outline Thickness", Range(0,1)) = 0.4
      _SpecColor ("Specular Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
   }
   
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } 
      
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag
         #pragma multi_compile_fwdbase 
         
         #pragma target 3.0 
 
         #include "UnityCG.cginc"
         #include "AutoLight.cginc"
         #include "Lighting.cginc"
         
         
         uniform sampler2D _MainTex;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float _Shininess;
         
      
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 tangent : TANGENT;
         };
         
         struct vertexOutput {
            float4 pos :         SV_POSITION;
            float4 posWorld :      TEXCOORD0;
            float3 tex :           TEXCOORD1;
            float3 normalDir :     TEXCOORD2;
            LIGHTING_COORDS(3,4)
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 			
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
                      
            output.tex = input.texcoord;
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = UnityObjectToClipPos(input.vertex);
          
            TRANSFER_VERTEX_TO_FRAGMENT(output);
            
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            
            float3 normalDirection = normalize(input.normalDir);
            float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb; 
            float4 nighttimeColor =  tex2D(_MainTex, input.tex);
            
            float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
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
               attenuation = 1.0 / distance; // calcula atenuaçao 
               lightDirection = normalize(vertexToLightSource);
            }
  
            float3 fragmentColor =  _LightColor0.rgb * ambientLighting; 

            if (attenuation * max(0.0, dot(normalDirection, lightDirection)) >= _DiffuseThreshold)
            { 
               fragmentColor = lerp(_LightColor0.rgb, nighttimeColor , 0.9);      
            }
 
            if (dot(normalDirection, lightDirection) > 0.0 
               && attenuation *  pow(max(0.0, dot(reflect(-lightDirection, normalDirection),viewDirection)), _Shininess) > 0.5) 
                
            {
               fragmentColor = _SpecColor.a * _LightColor0.rgb * _SpecColor.rgb + (1.0 - _SpecColor.a) * fragmentColor;
            }
            
            if (dot(viewDirection, normalDirection) < lerp(_UnlitOutlineThickness, _LitOutlineThickness, max(0.0, dot(normalDirection, lightDirection))))
            {
               fragmentColor = _LightColor0.rgb * _OutlineColor.rgb; 
            } 
            
               
             fragmentColor = fragmentColor * LIGHT_ATTENUATION(input);
            
             float4 daytimeColor = float4(fragmentColor, 1.0) ;  
              
             return lerp(nighttimeColor, daytimeColor, 0.3); 
         }
         ENDCG
      }
 
      Pass {      
         Tags { "LightMode" = "ForwardAdd" } 
         Blend SrcAlpha OneMinusSrcAlpha 
        
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag
         #pragma multi_compile_fwdbase 
         
         #pragma target 3.0 
 
         #include "UnityCG.cginc"
         #include "AutoLight.cginc"
         #include "Lighting.cginc"
         
         uniform sampler2D _MainTex;
         uniform float _DiffuseThreshold;
         uniform float4 _OutlineColor;
         uniform float _LitOutlineThickness;
         uniform float _UnlitOutlineThickness;
         uniform float _Shininess;
         
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 tangent : TANGENT;
         };
         
         struct vertexOutput {
            float4 pos :         SV_POSITION;
            float4 posWorld :      TEXCOORD0;
            float3 tex :           TEXCOORD1;
            float3 normalDir :     TEXCOORD2;
            LIGHTING_COORDS(3,4)
         };
 
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
 			 			
 			output.tex = input.texcoord;
            output.posWorld = mul(modelMatrix, input.vertex);
            output.pos = UnityObjectToClipPos(input.vertex);
            
            TRANSFER_VERTEX_TO_FRAGMENT(output);
            
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
 
 			float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb;
 			float4 nighttimeColor =  tex2D(_MainTex, input.tex); 
 
 			  
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.rgb);
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
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float4 fragmentColor = float4(0.0, 0.0, 0.0, 0.0);
            if (dot(normalDirection, lightDirection) > 0.0 
               && attenuation *  pow(max(0.0, dot(
               reflect(-lightDirection, normalDirection), 
               viewDirection)), _Shininess) > 0.5)  
            {
               fragmentColor = float4(_LightColor0.rgb, 1.0) * _SpecColor * float4(UNITY_LIGHTMODEL_AMBIENT.rgb, 1.0);
            }
            
             
             fragmentColor = fragmentColor * LIGHT_ATTENUATION(input);
           
            return lerp(nighttimeColor, fragmentColor, 0.4); 
         }
         ENDCG
      }
   } 
   Fallback "Specular"
}
