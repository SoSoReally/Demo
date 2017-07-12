// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.35 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.35;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:1,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7241-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:32471,y:32812,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;proporder:7241;pass:END;sub:END;*/

Shader "Shader Forge/deferred" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
		_Loop ("Loop",int ) = 0
		_Min("Mintance",float)= 0
		_Max("Maxtance",float)= 0
		//_Scale("Scale",Vector) =(0,0,0,0)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
			 "DisableBatching" = "True"
        }
		    Stencil
    {
        Comp Always
        Pass Replace
        Ref 128
    }
		CGINCLUDE

		#define DISTANCE_FUNCTION _DistanTarget

		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		float _Min;
		float _Loop;
		float _Max;
		float3 _Scale=float3(1,1,1);
		inline float mod(float a, float b)
		{
			return		frac(abs(a / b)) * abs(b);
		}


		inline float DistanTarget(float3 pos)
		{

		 pos*=0.3;
	     float tt = mod(_Time.y,1.5)/1.5;
		 float ss = pow(tt,.2)*0.5 + 0.5;
		 ss = 1.0 + ss*0.5*sin(tt*6.2831*3.0 + pos.y*0.5)*exp(-tt*4.0);
		 pos.xy *= float2(0.5,1.5) + ss*float2(0.5,-0.5);
		float x=pos.x;
		float y=pos.y;
	    float z=pos.z;
	
        float v=0.0;
	
		  v=sqrt(
			   pow(abs(x*2.5),2.0)
			   +z*z*0.9
					 +pow(
					abs(y*1.1-0.8*sqrt(sqrt(z*z+pow(abs(x/2.0),2.0)/(pow(abs(y+1.4),4.0)+0.001)))),2.0));
		return v-0.5;
		//return length(pos) -1;
		}
		struct Ray
		{
			float3 wordEndPos;
			float3 wordStartPos;
			float totalLength;
			float lastDistance;
			int maxloop;
			int loop;
			float3 dir;
			float min;
			float max;
			float3 normal;
			float3 WordNormal;
		};

		inline float3 To_Local(float3 wordPos)
		{
			return mul(unity_WorldToObject,float4(wordPos,1.0)).xyz;
		}
				inline float _DistanTarget(float3 pos)
		{
			return DistanTarget(To_Local(pos)*_Scale);
		}

		inline bool ShouldEnd(Ray ray)
		{
			if(ray.lastDistance<ray.min) return true;
			return false;
		}
		inline float3 GetNormal( float3 pos)
		{
			const float d = 0.0001;
		    float3 normal= (normalize(float3(
			DISTANCE_FUNCTION(pos + float3(  d, 0.0, 0.0)) - DISTANCE_FUNCTION(pos),
			DISTANCE_FUNCTION(pos + float3(0.0,   d, 0.0)) - DISTANCE_FUNCTION(pos),
			DISTANCE_FUNCTION(pos + float3(0.0, 0.0,   d)) - DISTANCE_FUNCTION(pos))));
		    return normal;
		}
		inline void RayMatching(inout Ray ray)
		{
			ray.wordEndPos = ray.wordStartPos;
			ray.totalLength = 0;
			ray.lastDistance = 0;
			for	(ray.loop = 0; ray.loop<ray.maxloop;++ray.loop)
			{
				ray.lastDistance  = _DistanTarget(ray.wordEndPos);
				ray.totalLength += ray.lastDistance;
				ray.wordEndPos += ray.dir*ray.lastDistance;
				if(ShouldEnd(ray))  break;
			}
			if(ray.lastDistance>ray.min) 
			{
			    discard;
			}
			else{
				if(ray.totalLength<ray.min)
				{
				 ray.normal = ray.WordNormal*0.5+0.5;
				}
				else
				{
				 ray.normal = GetNormal(ray.wordEndPos);
				}
			}

		}

		inline float3 CamerDir(float4 pos)
		{
		    pos.y *= -1.0;
		    pos.x *= (_ScreenParams.x / _ScreenParams.y);
		    pos.xy /= pos.w;
			

			float3 right = UNITY_MATRIX_V[0].xyz;
			float3 up = UNITY_MATRIX_V[1].xyz;
			float3 forward = -UNITY_MATRIX_V[2].xyz;
			float focalen = abs(UNITY_MATRIX_P[1][1]);
			return normalize(right* pos.x+up*pos.y+forward*focalen);
		}

		ENDCG
        Pass {
           // Name "DEFERRED"
            Tags {
                "LightMode"="Deferred"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_DEFERRED
		
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile ___ UNITY_HDR_ON
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
				float4 screenPos :TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
				
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
				o.pos = UnityObjectToClipPos(v.vertex );
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            void frag(
                VertexOutput i,
                out half4 outDiffuse : SV_Target0,
                out half4 outSpecSmoothness : SV_Target1,
                out half4 outNormal : SV_Target2,
                out half4 outEmission : SV_Target3 )
            {
			
			    Ray ray;
				UNITY_INITIALIZE_OUTPUT(Ray,ray);
				ray.wordStartPos = i.posWorld;
				ray.dir =  CamerDir(i.screenPos);
				//ray.dir= GetCameraDirection(i.screenPos);
				ray.min = _Min;
				ray.maxloop = _Loop;
				ray.max = _Max;
				ray.WordNormal = i.normalDir;
				RayMatching(ray);
				
				
                outDiffuse = half4( 0, 0, 0, 1 );
                outSpecSmoothness = half4(0,0,0,0);
				outNormal = half4(ray.normal,0);
               // outNormal = half4( normalDirection * 0.5 + 0.5, 1 );
			   /*
			    SurfaceOutput so;
				so.Albedo = _Color.rgb;
				so.Normal= ray.normal;
				so.Emission = half3(0,0,0);
				so.Specular = 0;
				so.Gloss = 0;
				so.Alpha =1;
				UnityLight light;
				light.color = _LightColor0;
				light.dir = _WorldSpaceLightPos0;
				fixed4 col= UnityLambertLight(so,light);
				*/
				fixed diff = max(0,dot(ray.normal,_WorldSpaceLightPos0));
				fixed3 col = _Color.rgb * _LightColor0 *diff;
				outEmission = fixed4( col,0);
               
                #ifndef UNITY_HDR_ON
                    outEmission.rgb = exp2(-outEmission.rgb);
                #endif
            }
            ENDCG
        }
		
    }
}
