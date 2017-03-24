// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable


Shader "Particle Dynamic Magic/VegetationV20" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Normals ("Normals", 2D) = "bump" {}
        _GlowColor ("Glow Color", Color) = (1,0.2391481,0.1102941,1)
        _BulgeScale ("Bulge Scale", Float ) = 0.2
        _BulgeShape ("Bulge Shape", Float ) = 5
        _GlowIntensity ("Glow Intensity", Float ) = 1.2
        _BulgeScale_copy ("Bulge Scale_copy", Float ) = 1.2
               
        _Params1("Parameters 1", Vector) = (1, 1, 0.8, 0)
        _Params2("Parameters 2", Vector) = (1, 1, 0.8, 0)
                
        _WaveControl1("Waves", Vector) = (1, 0.01, 0.001, 0)
        _TimeControl1("Time", Vector) = (1, 10, 0.02, 100)
        _OceanCenter("Ocean Center", Vector) = (0, 0, 0, 0)
        
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
                
    }
    SubShader {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 100
           // "RenderType"="Opaque"
       // }

        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
                //"LightMode" = "Vertex"
            }         
            Cull off
            Alphatest Greater 0.5
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH_PROBE ( defined (LIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
       //     #pragma exclude_renderers xbox360 ps3 flash 
            //#pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _BulgeScale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
            uniform float _GlowIntensity;
            uniform float _BulgeScale_copy;
            float3 _WaveControl1;
    float4 _TimeControl1;
    float3 _OceanCenter;
            //RIPLE PARAMS
              
    float4 _Params1;    
    float4 _Params2;

   uniform float4 _LightColor0;
    
    float _Cutoff;
    
    
    uniform float T;
	const float pi = 3.14159265;        
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
               float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 binormalDir : TEXCOORD3;
                float3 shLight : TEXCOORD4;
                float4 vPos : TEXCOORD5;
                float2 PassParam : TEXCOORD6;
                LIGHTING_COORDS(7,8)
        
            };
            
            
	VertexOutput vert (VertexInput v) {
                VertexOutput o ;
                o.uv0 = v.texcoord0;
                #if SHOULD_SAMPLE_SH_PROBE
                    o.shLight = ShadeSH9(float4(mul(unity_ObjectToWorld, float4(v.normal,0)).xyz * 1.0,1)) * 0.5;
                #endif
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                 
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                                
                o.PassParam.x = dist;
                o.PassParam.y = dist2;
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//*sin(dist + 1.5*dist*pi);
               float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                
                o.vPos = v.vertex;              
                
                if( o.uv0.y > 0.1){
                	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy));
				
				
				}		
			
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);                
                
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
    }
            
            

	fixed4 frag(VertexOutput i) : COLOR {

        i.normalDir = normalize(i.normalDir);
        float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
///////  Vectors:
        float3 _Normals_var = UnpackNormal(tex2D(_Normals,TRANSFORM_TEX(i.uv0, _Normals)));
                      
        float dist = i.PassParam.x;
        float dist2 = i.PassParam.y;
        
        float4 node_5027 = (_Time*_TimeControl1.x + _TimeEditor)+dist2*_TimeControl1.y;
        float4 node_50271 = (_Time + _TimeEditor);
                        node_5027 = 1;
                        node_50271=1;
                    
        float node_133 = i.uv0;// Panning gradient 
             
            
    	float4 t=node_50271;
		float d=length(tex2D(_Diffuse,t.xy).xyz);
    
     			float3 normalLocal = normalize(lerp(_Normals_var.rgb,float3(0,0,1),node_133));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
        		float NdotL = dot( normalDirection, lightDirection );
         		float attenuation = LIGHT_ATTENUATION(i);
   
        float3 attenColor = _LightColor0.xyz;
                float3 w = float3(node_133,node_133,node_133)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_133,node_133,node_133);
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = (forwardLight+backLight) * attenColor;
                #if SHOULD_SAMPLE_SH_PROBE
                    indirectDiffuse += i.shLight; // Per-Vertex Light Probes / Spherical harmonics
                #endif
    
////// Emissive:
                float3 emissive = ((_GlowColor.rgb)*_GlowIntensity*node_133);
               
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                 clip(_Diffuse_var.a - _Cutoff);

                float node_8608 = 1;
               float3 node_149 = (directDiffuse + indirectDiffuse) *lerp((_Diffuse_var.rgb),float3(node_8608,node_8608,node_8608),0);
                           
                float3 finalColor = emissive + node_149*_GlowColor.a;  
                
                finalColor = _Diffuse_var.w*finalColor - 0.05 ;              
               
                return fixed4(finalColor,_Diffuse_var.w);
                
  }
  ENDCG
}


Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles xbox360 ps3 flash 
           // #pragma target 3.0
            uniform float4 _TimeEditor;            
             
            uniform float _BulgeScale;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _GlowColor;
            uniform sampler2D _Normals; uniform float4 _Normals_ST;
            uniform float _BulgeShape;
            uniform float _GlowIntensity;
            uniform float _BulgeScale_copy;
            float3 _WaveControl1;
    		float4 _TimeControl1;
    		float3 _OceanCenter;
    		
    		uniform fixed _Cutoff;
            
            #ifndef LIGHTMAP_OFF
                // float4 unity_LightmapST;
                // sampler2D unity_Lightmap;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                
                o.vertexColor = v.vertexColor;
                             
                float dist = distance(_OceanCenter, float3(_WaveControl1.x*mul(unity_ObjectToWorld, v.vertex).y,_WaveControl1.y*mul(unity_ObjectToWorld, v.vertex).x,_WaveControl1.z*mul(unity_ObjectToWorld, v.vertex).z) );
                float dist2 = distance(_OceanCenter, float3(mul(unity_ObjectToWorld, v.vertex).y,mul(unity_ObjectToWorld, v.vertex).x*0.10,0.1*mul(unity_ObjectToWorld, v.vertex).z) );
                                
               o.uv0 = v.texcoord0;  
                
                float node_5027 = (_Time.y*_TimeControl1.x + _TimeEditor);//
               float node_133 = pow((abs((frac((o.uv0+node_5027*float2(0.2,0.1)).r)-0.5))*2.0),_BulgeShape);
                
                    o.normalDir  = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                
                if( o.uv0.y > 0.1){
                	v.vertex.xyz += (node_133*(_BulgeScale*sin(_TimeControl1.w*_Time.y +_TimeControl1.z + dist) )*v.normal*(v.normal*_BulgeScale_copy));
								
				}		
				
               
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                
                               
                
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float2 node_585 = i.uv0;
                float4 node_1 = tex2D(_Diffuse,TRANSFORM_TEX(node_585.rg, _Diffuse));
                clip(node_1.a - _Cutoff);                
             
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        
    }
    
    FallBack "Transparent/Cutout/Diffuse"
    
}