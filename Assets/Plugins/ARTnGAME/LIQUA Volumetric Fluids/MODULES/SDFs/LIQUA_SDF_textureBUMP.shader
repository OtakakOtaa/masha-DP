Shader "LIQUA/SDF_texture BUMP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}    
        _PlaneTex ("Plane Texture", 2D) = "white" {}   
        _CircleCol ("Circle Color", Color) = (1, 1, 1, 1) 
        _CircleRad ("Circle Radius", Range(0.0, 0.5)) = 0.45
        _Edge ("Edge", Range(-0.7, 0.7)) = 0.0

		_powerInside("powerInside",Float) = 2.0
		_offsetInside ("_offsetInside", Vector) = (0, 0, 0, 0)

		//v0.2
		_BumpMap ("Normal Map", 2D) = "bump" {}
        _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
        _Shininess ("Shininess", Float) = 10

		_SunIntensity ("Sun Intensity", Float) = 1
		_SunColor ("Sun Color", Color) = (1,1,1,1) 
		_SunDirection ("Sun Direction", Vector) = (1,1,1,1)
		normalIntensity  ("normal Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			    float4 tangent : TANGENT;
            };

            struct v2f
            {               
                float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float2 tex  : TEXCOORD1;
				float3 tangentWorld : TEXCOORD2;  
				float3 normalWorld : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
                float3 hitPos : TEXCOORD5; 
            };

            sampler2D _MainTex;
            sampler2D _PlaneTex;
            float4 _MainTex_ST;
            float4 _CircleCol;
            float _Edge;
            float _CircleRad;

			//v0.1
			float _powerInside;
			float4 _offsetInside;

			//v0.2
			uniform float _SunIntensity;
			uniform float4 _SunColor; 
			uniform float4 _SunDirection;
		    // color of light source (from "Lighting.cginc")
		    // User-specified properties
		    uniform sampler2D _BumpMap;   
		    uniform float4 _BumpMap_ST;
		    uniform float4 _Color; 
		    uniform float4 _SpecColor; 
		    uniform float _Shininess;
			float normalIntensity;

            v2f vert (appdata input)
            {
                v2f output;
                output.pos = UnityObjectToClipPos(input.vertex);

				//v0.2
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				output.tangentWorld = normalize(
					mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
				output.normalWorld = normalize(
					mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				output.binormalWorld = normalize(
					cross(output.normalWorld, output.tangentWorld) 
					* input.tangent.w); // tangent.w is specific to Unity

				output.posWorld = mul(modelMatrix, input.vertex);

                output.tex  = TRANSFORM_TEX(input.texcoord, _MainTex);
                output.hitPos = input.vertex; 
                return output;
            }

            #define MAX_MARCHING_STEPS 50
            #define MAX_DISTANCE 120.0
            #define SURFACE_DISTANCE 0.001

            float planeSDF(float3 ray_position)
            {     
                float plane = ray_position.y - _Edge;
                return plane;
            }

            float sphereCasting(float3 ray_origin, float3 ray_direction)
            {     
                float distance_origin = 0;
                for(int i = 0; i < MAX_MARCHING_STEPS; i++)
                {
                    // ray_position se refiere a cada punto de encuentro en la marcha
                    float3 ray_position = ray_origin + ray_direction * distance_origin;
                    float distance_scene = planeSDF(ray_position);
                    distance_origin += distance_scene;

                    if(distance_scene < SURFACE_DISTANCE || distance_origin > MAX_DISTANCE) 
                        break;
                }

                return distance_origin;
            }            

            fixed4 frag (v2f input, bool face : SV_isFrontFace) : SV_Target
            {

				//v0.2
				float4 encodedNormal = tex2D(_BumpMap, _BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw)*normalIntensity;
				float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g - 1.0, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
				float3x3 local2WorldTranspose = float3x3(input.tangentWorld,input.binormalWorld,input.normalWorld);
				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;
				float attenuation;

				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{ 
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);	//////
				} 
				else // point or spot light
				{
					float3 vertexToLightSource = 
					   _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}

				 float3 ambientLighting = 
					UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

				 float3 diffuseReflection = 
					attenuation * _SunColor.rgb * _Color.rgb * _SunIntensity
					* max(0.0, dot(normalDirection, lightDirection));

				 float3 specularReflection;
				 if (dot(normalDirection, lightDirection) < 0.0) 
					// light source on the wrong side?
				 {
					specularReflection = float3(0.0, 0.0, 0.0); 
					// no specular reflection
				 }
				 else // light source on the right side
				 {
					specularReflection = attenuation * _SunColor.rgb * _SunIntensity
					   * _SpecColor.rgb * pow(max(0.0, dot(
					   reflect(-lightDirection, normalDirection), 
					   viewDirection)), _Shininess);
				 }
				 float4 result =  float4(ambientLighting + diffuseReflection 
					+ specularReflection, 1.0);


                float4 col = tex2D(_MainTex, input.tex);

                float3 ray_origin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                float3 ray_direction = normalize(input.hitPos - ray_origin); 

                float t = sphereCasting(ray_origin, ray_direction);
                float4 planeCol = 0;
                float4 circleCol = 0;

                if(t < MAX_DISTANCE)  
                {
                    float3 p = ray_origin + ray_direction * t;  
                    float2 uv_p = p.xz;

                    float l = pow(-abs(_Edge), 2) + pow(-abs(_Edge) - 1, 2);
                    float c = length(uv_p);
                    
                    circleCol = (smoothstep(c - 0.01, c + 0.01, _CircleRad - abs(pow(_Edge * (l * 0.5), 2))));
                    //planeCol = tex2D(_PlaneTex, (uv_p * (1 + abs(pow(_Edge * l, _powerInside)))) - 0.5);
					planeCol = tex2D(_PlaneTex, (uv_p * (1 + abs(pow(_Edge * l, _powerInside)))) - float2(0.5, 0.5) + _offsetInside.xy);
                    planeCol *= circleCol;
                    planeCol += (1 - circleCol) * _CircleCol;
                }

                if(input.hitPos.y > _Edge) discard;
                    
                return face ? col+col*result : planeCol+planeCol*result;
            }
            ENDCG
        }
    }
}
