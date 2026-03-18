Shader "SimuranS/Ripple"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture Sample", 2D) = "white" {}
		_Tilling("Tilling", Vector) = (3,3,0,0)
		_Step("Step", Float) = 1
		_Time_noise("Time_noise", Float) = 0.75
		[HDR]_Color0("Color", Color) = (1,1,1,1)
		_Step2("Step2", Vector) = (0.1,0.94,0,0)
		_SphereCenter("SphereCenter", Vector) = (0,0,-1.51,0)
		_impactsize("impact size", Range( 0 , 5)) = 1
		_dampl("Saturation", Float) = 15

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend One One
			ColorMask RGB
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


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
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
				uniform float3 _SphereCenter;
				uniform float _impactsize;
				uniform float _dampl;
				uniform sampler2D _TextureSample0;
				SamplerState sampler_TextureSample0;
				uniform float2 _Tilling;
				uniform float _Step;
				uniform float _Time_noise;
				uniform float2 _Step2;
				uniform float4 _Color0;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.ase_texcoord3.xyz = ase_worldPos;
					
					
					//setting value to unused interpolator channels and avoid initialization warnings
					o.ase_texcoord3.w = 0;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
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

					float3 ase_worldPos = i.ase_texcoord3.xyz;
					float2 texCoord2 = i.texcoord.xy * _Tilling + float2( 0,0 );
					float4 tex2DNode1 = tex2D( _TextureSample0, texCoord2 );
					float temp_output_14_0 = (_Step2.x + (sin( ( _Time.y * _Time_noise ) ) - -1.0) * (_Step2.y - _Step2.x) / (1.0 - -1.0));
					

					fixed4 col = ( ( 1.0 - saturate( pow( ( ( distance( _SphereCenter , ase_worldPos ) / 1.0 ) * _impactsize ) , _dampl ) ) ) * ( ( step( tex2DNode1.b , ( _Step + temp_output_14_0 ) ) - step( tex2DNode1.b , temp_output_14_0 ) ) * _Color0 * i.color * i.color.a * tex2DNode1.a ) );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18500
424;65;1426;842;750.0721;51.80556;2.426994;True;True
Node;AmplifyShaderEditor.TimeNode;8;-1914.982,182.1453;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-1825.982,410.1453;Inherit;False;Property;_Time_noise;Time_noise;3;0;Create;True;0;0;False;0;False;0.75;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1609.982,273.1453;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;22;-851.5003,1110.604;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;24;-526.6664,883.6046;Inherit;False;Property;_SphereCenter;SphereCenter;6;0;Create;True;0;0;False;0;False;0,0,-1.51;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;26;-189.5328,1199.917;Inherit;False;Constant;_Float1;Float 1;6;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;23;-225.699,949.8322;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;15;-1164.982,494.1453;Inherit;False;Property;_Step2;Step2;5;0;Create;True;0;0;False;0;False;0.1,0.94;0.1,0.7;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;3;-1884.982,-109.8547;Inherit;False;Property;_Tilling;Tilling;1;0;Create;True;0;0;False;0;False;3,3;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;13;-1386.982,295.1453;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;25;9.467163,1013.917;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1584.51,-64.00531;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;14;-849.9822,416.1453;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-836.4653,263.6405;Inherit;False;Property;_Step;Step;2;0;Create;True;0;0;False;0;False;1;45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;13.54415,1187.665;Inherit;False;Property;_impactsize;impact size;7;0;Create;True;0;0;False;0;False;1;1.99;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;298.4672,1256.917;Inherit;False;Property;_dampl;dampl;8;0;Create;True;0;0;False;0;False;15;45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;243.4672,1037.917;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-559.6374,308.0995;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1300.517,-91.06622;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;-1;None;d701932bd6f0b24449725f74835b381b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;16;81.63171,374.0645;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;29;476.4672,1022.917;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;4;-77.53886,51.39928;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;456.433,202.7834;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;519.6461,532.5027;Inherit;False;Property;_Color0;Color 0;4;1;[HDR];Create;True;0;0;False;0;False;1,1,1,1;23.96863,23.96863,23.96863,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;20;609.9414,-67.85687;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;31;758.4672,1032.917;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;32;946.4672,1038.917;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;1056.186,390.4481;Inherit;True;5;5;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;1298.128,711.144;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1831.321,455.5771;Float;False;True;-1;2;ASEMaterialInspector;0;7;SimuranS/Ripple;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;True;4;1;False;-1;1;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;True;0;False;-1;True;True;True;True;False;0;False;-1;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;9;0;8;2
WireConnection;9;1;10;0
WireConnection;23;0;24;0
WireConnection;23;1;22;0
WireConnection;13;0;9;0
WireConnection;25;0;23;0
WireConnection;25;1;26;0
WireConnection;2;0;3;0
WireConnection;14;0;13;0
WireConnection;14;3;15;1
WireConnection;14;4;15;2
WireConnection;27;0;25;0
WireConnection;27;1;28;0
WireConnection;5;0;6;0
WireConnection;5;1;14;0
WireConnection;1;1;2;0
WireConnection;16;0;1;3
WireConnection;16;1;14;0
WireConnection;29;0;27;0
WireConnection;29;1;30;0
WireConnection;4;0;1;3
WireConnection;4;1;5;0
WireConnection;17;0;4;0
WireConnection;17;1;16;0
WireConnection;31;0;29;0
WireConnection;32;0;31;0
WireConnection;19;0;17;0
WireConnection;19;1;18;0
WireConnection;19;2;20;0
WireConnection;19;3;20;4
WireConnection;19;4;1;4
WireConnection;33;0;32;0
WireConnection;33;1;19;0
WireConnection;0;0;33;0
ASEEND*/
//CHKSM=17CAAD3F9C20F93B657E3DC85CCFC99FC7778ABB