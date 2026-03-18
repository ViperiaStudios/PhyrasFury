Shader "SimuranS/Energy Shield"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		_Divide("Divide", Float) = 0.25
		_DivideMix("Divide Mix", Range( 0 , 50)) = 3.532351
		[Toggle(_STOPONOFF_ON)] _StopONOFF("Stop ON/OFF", Float) = 0
		[Toggle(_SPHEREMASK_ON)] _sphereMask("sphereMask", Float) = 1
		[Toggle(_TEXCOORDVERTEXPOS_ON)] _TexCoordVertexPos("TexCoord/VertexPos", Float) = 0
		_SphereCenter("SphereCenter", Vector) = (0.51,0.45,1.15,0)
		[HDR]_Depthfade("Contact Effect Color", Color) = (1,1,1,1)
		_DethDistance("Contact Effect Size", Float) = 0.2
		_radius("radius", Range( 0 , 3)) = 1
		[Toggle(_ONCIRCLEOFPLANE_ON)] _OnCircleOfPlane("OnCircle/OfPlane", Float) = 0

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGBA
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR
				#pragma shader_feature _STOPONOFF_ON
				#pragma shader_feature _TEXCOORDVERTEXPOS_ON
				#pragma shader_feature _SPHEREMASK_ON
				#pragma shader_feature _ONCIRCLEOFPLANE_ON


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float3 ase_normal : NORMAL;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
					float3 ase_normal : NORMAL;
					float4 ase_texcoord4 : TEXCOORD4;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _Divide;
				uniform float _DivideMix;
				uniform float4 _Color0;
				uniform sampler2D _TextureSample0;
				SamplerState sampler_TextureSample0;
				uniform float3 _SphereCenter;
				uniform float _radius;
				uniform float4 _Depthfade;
				uniform float4 _CameraDepthTexture_TexelSize;
				uniform float _DethDistance;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float2 texCoord5 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					#ifdef _TEXCOORDVERTEXPOS_ON
					float3 staticSwitch10 = ( float3(0.5,0.5,0.5) + ase_worldPos );
					#else
					float3 staticSwitch10 = float3( texCoord5 ,  0.0 );
					#endif
					float3 temp_output_25_0 = ( sin( ( ( staticSwitch10 + _Time.w ) / _Divide ) ) / _DivideMix );
					#ifdef _STOPONOFF_ON
					float3 staticSwitch39 = max( temp_output_25_0 , float3( 0,0,0 ) );
					#else
					float3 staticSwitch39 = temp_output_25_0;
					#endif
					
					o.ase_texcoord3.xyz = ase_worldPos;
					float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
					float4 screenPos = ComputeScreenPos(ase_clipPos);
					o.ase_texcoord4 = screenPos;
					
					o.ase_normal = v.ase_normal;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;

					v.vertex.xyz += ( v.ase_normal * staticSwitch39 );
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float4 tex2DNode23 = tex2D( _TextureSample0, i.texcoord.xy );
					float3 ase_worldPos = i.ase_texcoord3.xyz;
					#ifdef _ONCIRCLEOFPLANE_ON
					float3 staticSwitch114 = ( ase_worldPos + i.ase_normal );
					#else
					float3 staticSwitch114 = ase_worldPos;
					#endif
					#ifdef _SPHEREMASK_ON
					float staticSwitch86 = ( 1.0 - saturate( pow( ( ( distance( _SphereCenter , staticSwitch114 ) / 1.0 ) * _radius ) , 1500.0 ) ) );
					#else
					float staticSwitch86 = 1.0;
					#endif
					float4 screenPos = i.ase_texcoord4;
					float4 ase_screenPosNorm = screenPos / screenPos.w;
					ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
					float screenDepth78 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
					float distanceDepth78 = abs( ( screenDepth78 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DethDistance ) );
					float clampResult80 = clamp( ( 1.0 - distanceDepth78 ) , 0.0 , 1.0 );
					

					fixed4 col = ( ( ( i.color * ( _Color0 * tex2DNode23 ) * i.color.a * _Color0.a * tex2DNode23.a ) * staticSwitch86 ) + ( _Depthfade * clampResult80 ) );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}