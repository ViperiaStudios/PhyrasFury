Shader "SimuranS/Fresnel"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		[HDR]_Color1("Color 1", Color) = (0,0.7530308,1,1)
		_Divide("Divide", Float) = 0.25
		_DivideMix("DivideMix", Range( 0 , 50)) = 3.532351
		[Toggle(_STOP_ON)] _stop("stop", Float) = 0
		[Toggle(_TEXCOORDVERTEXPOS_ON)] _TexCoordVertexPos("TexCoord/VertexPos", Float) = 0
		_VectorXYZ("Vector XYZ", Vector) = (1,2,0,0)
		[Toggle(_SPHEREMASK_ON)] _SphereMask("SphereMask", Float) = 0

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask RGBA
			Cull Back
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
				#pragma shader_feature _STOP_ON
				#pragma shader_feature _TEXCOORDVERTEXPOS_ON
				#pragma shader_feature _SPHEREMASK_ON


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
					float4 ase_texcoord4 : TEXCOORD4;
					float3 ase_normal : NORMAL;
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
				uniform float4 _Color1;
				uniform float3 _VectorXYZ;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float2 texCoord77 = v.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					#ifdef _TEXCOORDVERTEXPOS_ON
					float3 staticSwitch102 = ( float3(0.5,0.5,0.5) + ase_worldPos );
					#else
					float3 staticSwitch102 = float3( texCoord77 ,  0.0 );
					#endif
					float3 temp_output_73_0 = ( sin( ( ( staticSwitch102 + _Time.w ) / _Divide ) ) / _DivideMix );
					#ifdef _STOP_ON
					float3 staticSwitch86 = max( temp_output_73_0 , float3( 0,0,0 ) );
					#else
					float3 staticSwitch86 = temp_output_73_0;
					#endif
					
					o.ase_texcoord3.xyz = ase_worldPos;
					float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
					o.ase_texcoord4.xyz = ase_worldNormal;
					
					o.ase_normal = v.ase_normal;
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;
					o.ase_texcoord4.w = 0;

					v.vertex.xyz += ( v.ase_normal * staticSwitch86 );
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

					float4 tex2DNode2 = tex2D( _TextureSample0, i.texcoord.xy );
					float3 ase_worldPos = i.ase_texcoord3.xyz;
					float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
					ase_worldViewDir = normalize(ase_worldViewDir);
					float3 ase_worldNormal = i.ase_texcoord4.xyz;
					float fresnelNdotV5 = dot( ase_worldNormal, ase_worldViewDir );
					float fresnelNode5 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV5, 1.0 ) );
					float3 worldToObj147 = mul( unity_WorldToObject, float4( i.ase_normal, 1 ) ).xyz;
					#ifdef _SPHEREMASK_ON
					float staticSwitch159 = ( 1.0 - saturate( pow( ( ( distance( _VectorXYZ , worldToObj147 ) / 1.0 ) * 1.0 ) , 15.0 ) ) );
					#else
					float staticSwitch159 = 1.0;
					#endif
					

					fixed4 col = ( ( ( i.color * ( _Color0 * tex2DNode2 ) * i.color.a * _Color0.a * tex2DNode2.a ) + ( _Color1 * fresnelNode5 * _Color1.a ) ) * staticSwitch159 );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}