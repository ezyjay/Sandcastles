// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ClayxelFoliageShaderURP"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Metallic("Metallic", Float) = 0
		_ClayxelSize("ClayxelSize", Float) = 0
		_Fuzz("Fuzz", Float) = 0
		_NormalOrient("NormalOrient", Float) = 0
		_Smoothness1("Smoothness", Float) = 0
		[NoScaleOffset]_MainTex("Texture", 2D) = "white" {}
		[HDR]_Emission("Emission", Color) = (1,1,1,1)
		_randomize("randomize", Float) = 0

	}

	SubShader
	{
		LOD 0

		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
		Cull Back
		HLSLINCLUDE
		#pragma target 4.5
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#include "../../Resources/clayxelSRPUtils.cginc"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float _ClayxelSize;
			float _NormalOrient;
			float _randomize;
			float4 _Emission;
			float _Metallic;
			float _Smoothness1;
			float _Fuzz;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				uint ase_vertexID : SV_VertexID;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float localclayxelComputeVertexURP100 = ( 0.0 );
				int vertexId100 = v.ase_vertexID;
				float3 vertexPosition100 = float3( 0,0,0 );
				float3 vertexNormal100 = float3( 0,0,0 );
				float clayxelSize100 = _ClayxelSize;
				float normalOrient100 = ( _NormalOrient * 0.45 );
				float3 color = float3(0, 0, 0);
				float4 tex = float4(0, 0, 0, 0);
				clayxelVertNormalBlend(vertexId100 , clayxelSize100, normalOrient100, tex, color, vertexPosition100, vertexNormal100);
				v.vertex.w=1;
				v.ase_texcoord = tex;
				#if defined(SHADERPASS_FORWARD) 
				v.ase_color = float4(color, 1);
				#endif
				
				float localclayxelGetPointCloud34 = ( 0.0 );
				int vertexId34 = v.ase_vertexID;
				float3 pointCenter34 = float3( 0,0,0 );
				float3 pointNormal34 = float3( 0,0,0 );
				float3 pointColor34 = float3( 0,0,0 );
				float3 gridPoint34 = float3( 0,0,0 );
				clayxelGetPointCloud(vertexId34, gridPoint34, pointColor34, pointCenter34, pointNormal34);
				float3 break70 = cross( pointCenter34 , pointNormal34 );
				float2 appendResult69 = (float2(break70.x , break70.y));
				float dotResult4_g3 = dot( appendResult69 , float2( 12.9898,78.233 ) );
				float lerpResult10_g3 = lerp( 0.0 , 1000.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
				float cos29 = cos( ( lerpResult10_g3 * _randomize ) );
				float sin29 = sin( ( lerpResult10_g3 * _randomize ) );
				float2 rotator29 = mul( v.ase_texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
				float2 vertexToFrag91 = rotator29;
				o.ase_texcoord7.xy = vertexToFrag91;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord7.zw = v.ase_texcoord.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = vertexPosition100;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = vertexNormal100;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				float3 WorldNormal = normalize( IN.tSpace0.xyz );
				float3 WorldTangent = IN.tSpace1.xyz;
				float3 WorldBiTangent = IN.tSpace2.xyz;
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				#if SHADER_HINT_NICE_QUALITY
					WorldViewDirection = SafeNormalize( WorldViewDirection );
				#endif

				float2 vertexToFrag91 = IN.ase_texcoord7.xy;
				float4 tex2DNode11 = tex2D( _MainTex, vertexToFrag91 );
				
				float dotResult4_g4 = dot( IN.ase_texcoord7.zw , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , tex2DNode11.a , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float ifLocalVar27 = 0;
				UNITY_BRANCH 
				if( tex2DNode11.a > _Fuzz )
				ifLocalVar27 = lerpResult10_g4;
				
				float3 Albedo = ( IN.ase_color * tex2DNode11 ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = _Emission.rgb;
				float3 Specular = 0.5;
				float Metallic = _Metallic;
				float Smoothness = _Smoothness1;
				float Occlusion = 1;
				float Alpha = ifLocalVar27;
				float AlphaClipThreshold = _Fuzz;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				
				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					inputData.normalWS = normalize(TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal )));
				#else
					#if !SHADER_HINT_NICE_QUALITY
						inputData.normalWS = WorldNormal;
					#else
						inputData.normalWS = normalize( WorldNormal );
					#endif
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.lightmapUVOrVertexSH.xyz, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, WorldNormal ).xyz * ( 1.0 / ( ScreenPos.z + 1.0 ) ) * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
					projScreenPos.xy += cameraRefraction;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "../../Resources/clayxelSRPUtils.cginc"


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float _ClayxelSize;
			float _NormalOrient;
			float _randomize;
			float4 _Emission;
			float _Metallic;
			float _Smoothness1;
			float _Fuzz;
			CBUFFER_END


			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			float3 _LightDirection;

			VertexOutput ShadowPassVertex( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float localclayxelComputeVertexURP100 = ( 0.0 );
				int vertexId100 = v.ase_vertexID;
				float3 vertexPosition100 = float3( 0,0,0 );
				float3 vertexNormal100 = float3( 0,0,0 );
				float clayxelSize100 = _ClayxelSize;
				float normalOrient100 = ( _NormalOrient * 0.45 );
				float3 color = float3(0, 0, 0);
				float4 tex = float4(0, 0, 0, 0);
				clayxelVertNormalBlend(vertexId100 , clayxelSize100, normalOrient100, tex, color, vertexPosition100, vertexNormal100);
				v.vertex.w=1;
				v.ase_texcoord = tex;
				#if defined(SHADERPASS_FORWARD) 
				v.ase_color = float4(color, 1);
				#endif
				
				float localclayxelGetPointCloud34 = ( 0.0 );
				int vertexId34 = v.ase_vertexID;
				float3 pointCenter34 = float3( 0,0,0 );
				float3 pointNormal34 = float3( 0,0,0 );
				float3 pointColor34 = float3( 0,0,0 );
				float3 gridPoint34 = float3( 0,0,0 );
				clayxelGetPointCloud(vertexId34, gridPoint34, pointColor34, pointCenter34, pointNormal34);
				float3 break70 = cross( pointCenter34 , pointNormal34 );
				float2 appendResult69 = (float2(break70.x , break70.y));
				float dotResult4_g3 = dot( appendResult69 , float2( 12.9898,78.233 ) );
				float lerpResult10_g3 = lerp( 0.0 , 1000.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
				float cos29 = cos( ( lerpResult10_g3 * _randomize ) );
				float sin29 = sin( ( lerpResult10_g3 * _randomize ) );
				float2 rotator29 = mul( v.ase_texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
				float2 vertexToFrag91 = rotator29;
				o.ase_texcoord2.xy = vertexToFrag91;
				
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = vertexPosition100;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = vertexNormal100;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			half4 ShadowPassFragment(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 vertexToFrag91 = IN.ase_texcoord2.xy;
				float4 tex2DNode11 = tex2D( _MainTex, vertexToFrag91 );
				float dotResult4_g4 = dot( IN.ase_texcoord2.zw , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , tex2DNode11.a , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float ifLocalVar27 = 0;
				UNITY_BRANCH 
				if( tex2DNode11.a > _Fuzz )
				ifLocalVar27 = lerpResult10_g4;
				
				float Alpha = ifLocalVar27;
				float AlphaClipThreshold = _Fuzz;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "../../Resources/clayxelSRPUtils.cginc"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float _ClayxelSize;
			float _NormalOrient;
			float _randomize;
			float4 _Emission;
			float _Metallic;
			float _Smoothness1;
			float _Fuzz;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float localclayxelComputeVertexURP100 = ( 0.0 );
				int vertexId100 = v.ase_vertexID;
				float3 vertexPosition100 = float3( 0,0,0 );
				float3 vertexNormal100 = float3( 0,0,0 );
				float clayxelSize100 = _ClayxelSize;
				float normalOrient100 = ( _NormalOrient * 0.45 );
				float3 color = float3(0, 0, 0);
				float4 tex = float4(0, 0, 0, 0);
				clayxelVertNormalBlend(vertexId100 , clayxelSize100, normalOrient100, tex, color, vertexPosition100, vertexNormal100);
				v.vertex.w=1;
				v.ase_texcoord = tex;
				#if defined(SHADERPASS_FORWARD) 
				v.ase_color = float4(color, 1);
				#endif
				
				float localclayxelGetPointCloud34 = ( 0.0 );
				int vertexId34 = v.ase_vertexID;
				float3 pointCenter34 = float3( 0,0,0 );
				float3 pointNormal34 = float3( 0,0,0 );
				float3 pointColor34 = float3( 0,0,0 );
				float3 gridPoint34 = float3( 0,0,0 );
				clayxelGetPointCloud(vertexId34, gridPoint34, pointColor34, pointCenter34, pointNormal34);
				float3 break70 = cross( pointCenter34 , pointNormal34 );
				float2 appendResult69 = (float2(break70.x , break70.y));
				float dotResult4_g3 = dot( appendResult69 , float2( 12.9898,78.233 ) );
				float lerpResult10_g3 = lerp( 0.0 , 1000.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
				float cos29 = cos( ( lerpResult10_g3 * _randomize ) );
				float sin29 = sin( ( lerpResult10_g3 * _randomize ) );
				float2 rotator29 = mul( v.ase_texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
				float2 vertexToFrag91 = rotator29;
				o.ase_texcoord2.xy = vertexToFrag91;
				
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = vertexPosition100;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = vertexNormal100;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 vertexToFrag91 = IN.ase_texcoord2.xy;
				float4 tex2DNode11 = tex2D( _MainTex, vertexToFrag91 );
				float dotResult4_g4 = dot( IN.ase_texcoord2.zw , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , tex2DNode11.a , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float ifLocalVar27 = 0;
				UNITY_BRANCH 
				if( tex2DNode11.a > _Fuzz )
				ifLocalVar27 = lerpResult10_g4;
				
				float Alpha = ifLocalVar27;
				float AlphaClipThreshold = _Fuzz;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "../../Resources/clayxelSRPUtils.cginc"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float _ClayxelSize;
			float _NormalOrient;
			float _randomize;
			float4 _Emission;
			float _Metallic;
			float _Smoothness1;
			float _Fuzz;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				uint ase_vertexID : SV_VertexID;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float localclayxelComputeVertexURP100 = ( 0.0 );
				int vertexId100 = v.ase_vertexID;
				float3 vertexPosition100 = float3( 0,0,0 );
				float3 vertexNormal100 = float3( 0,0,0 );
				float clayxelSize100 = _ClayxelSize;
				float normalOrient100 = ( _NormalOrient * 0.45 );
				float3 color = float3(0, 0, 0);
				float4 tex = float4(0, 0, 0, 0);
				clayxelVertNormalBlend(vertexId100 , clayxelSize100, normalOrient100, tex, color, vertexPosition100, vertexNormal100);
				v.vertex.w=1;
				v.ase_texcoord = tex;
				#if defined(SHADERPASS_FORWARD) 
				v.ase_color = float4(color, 1);
				#endif
				
				float localclayxelGetPointCloud34 = ( 0.0 );
				int vertexId34 = v.ase_vertexID;
				float3 pointCenter34 = float3( 0,0,0 );
				float3 pointNormal34 = float3( 0,0,0 );
				float3 pointColor34 = float3( 0,0,0 );
				float3 gridPoint34 = float3( 0,0,0 );
				clayxelGetPointCloud(vertexId34, gridPoint34, pointColor34, pointCenter34, pointNormal34);
				float3 break70 = cross( pointCenter34 , pointNormal34 );
				float2 appendResult69 = (float2(break70.x , break70.y));
				float dotResult4_g3 = dot( appendResult69 , float2( 12.9898,78.233 ) );
				float lerpResult10_g3 = lerp( 0.0 , 1000.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
				float cos29 = cos( ( lerpResult10_g3 * _randomize ) );
				float sin29 = sin( ( lerpResult10_g3 * _randomize ) );
				float2 rotator29 = mul( v.ase_texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
				float2 vertexToFrag91 = rotator29;
				o.ase_texcoord2.xy = vertexToFrag91;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = vertexPosition100;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = vertexNormal100;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 vertexToFrag91 = IN.ase_texcoord2.xy;
				float4 tex2DNode11 = tex2D( _MainTex, vertexToFrag91 );
				
				float dotResult4_g4 = dot( IN.ase_texcoord2.zw , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , tex2DNode11.a , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float ifLocalVar27 = 0;
				UNITY_BRANCH 
				if( tex2DNode11.a > _Fuzz )
				ifLocalVar27 = lerpResult10_g4;
				
				
				float3 Albedo = ( IN.ase_color * tex2DNode11 ).rgb;
				float3 Emission = _Emission.rgb;
				float Alpha = ifLocalVar27;
				float AlphaClipThreshold = _Fuzz;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero , One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_ABSOLUTE_VERTEX_POS 1
			#define _EMISSION
			#define _ALPHATEST_ON 1
			#define ASE_SRP_VERSION 999999

			#pragma enable_d3d11_debug_symbols
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#include "../../Resources/clayxelSRPUtils.cginc"


			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float _ClayxelSize;
			float _NormalOrient;
			float _randomize;
			float4 _Emission;
			float _Metallic;
			float _Smoothness1;
			float _Fuzz;
			CBUFFER_END


			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				uint ase_vertexID : SV_VertexID;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float localclayxelComputeVertexURP100 = ( 0.0 );
				int vertexId100 = v.ase_vertexID;
				float3 vertexPosition100 = float3( 0,0,0 );
				float3 vertexNormal100 = float3( 0,0,0 );
				float clayxelSize100 = _ClayxelSize;
				float normalOrient100 = ( _NormalOrient * 0.45 );
				float3 color = float3(0, 0, 0);
				float4 tex = float4(0, 0, 0, 0);
				clayxelVertNormalBlend(vertexId100 , clayxelSize100, normalOrient100, tex, color, vertexPosition100, vertexNormal100);
				v.vertex.w=1;
				v.ase_texcoord = tex;
				#if defined(SHADERPASS_FORWARD) 
				v.ase_color = float4(color, 1);
				#endif
				
				float localclayxelGetPointCloud34 = ( 0.0 );
				int vertexId34 = v.ase_vertexID;
				float3 pointCenter34 = float3( 0,0,0 );
				float3 pointNormal34 = float3( 0,0,0 );
				float3 pointColor34 = float3( 0,0,0 );
				float3 gridPoint34 = float3( 0,0,0 );
				clayxelGetPointCloud(vertexId34, gridPoint34, pointColor34, pointCenter34, pointNormal34);
				float3 break70 = cross( pointCenter34 , pointNormal34 );
				float2 appendResult69 = (float2(break70.x , break70.y));
				float dotResult4_g3 = dot( appendResult69 , float2( 12.9898,78.233 ) );
				float lerpResult10_g3 = lerp( 0.0 , 1000.0 , frac( ( sin( dotResult4_g3 ) * 43758.55 ) ));
				float cos29 = cos( ( lerpResult10_g3 * _randomize ) );
				float sin29 = sin( ( lerpResult10_g3 * _randomize ) );
				float2 rotator29 = mul( v.ase_texcoord.xy - float2( 0.5,0.5 ) , float2x2( cos29 , -sin29 , sin29 , cos29 )) + float2( 0.5,0.5 );
				float2 vertexToFrag91 = rotator29;
				o.ase_texcoord2.xy = vertexToFrag91;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2.zw = v.ase_texcoord.xy;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = vertexPosition100;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = vertexNormal100;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 vertexToFrag91 = IN.ase_texcoord2.xy;
				float4 tex2DNode11 = tex2D( _MainTex, vertexToFrag91 );
				
				float dotResult4_g4 = dot( IN.ase_texcoord2.zw , float2( 12.9898,78.233 ) );
				float lerpResult10_g4 = lerp( 0.0 , tex2DNode11.a , frac( ( sin( dotResult4_g4 ) * 43758.55 ) ));
				float ifLocalVar27 = 0;
				UNITY_BRANCH 
				if( tex2DNode11.a > _Fuzz )
				ifLocalVar27 = lerpResult10_g4;
				
				
				float3 Albedo = ( IN.ase_color * tex2DNode11 ).rgb;
				float Alpha = ifLocalVar27;
				float AlphaClipThreshold = _Fuzz;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	
	
	
}
/*ASEBEGIN
Version=18000
783;319;1413;610;1231.518;97.48544;1.399923;True;False
Node;AmplifyShaderEditor.VertexIdVariableNode;35;-3236.483,-243.2999;Inherit;False;0;1;INT;0
Node;AmplifyShaderEditor.CustomExpressionNode;34;-3064.402,-193.1563;Inherit;False;clayxelGetPointCloud(vertexId, gridPoint, pointColor, pointCenter, pointNormal)@;7;True;5;False;vertexId;INT;0;In;;Inherit;False;True;pointCenter;FLOAT3;0,0,0;Out;;Inherit;False;True;pointNormal;FLOAT3;0,0,0;Out;;Inherit;False;True;pointColor;FLOAT3;0,0,0;Out;;Inherit;False;True;gridPoint;FLOAT3;0,0,0;Out;;Inherit;False;clayxelGetPointCloud;False;False;0;6;0;FLOAT;0;False;1;INT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;5;FLOAT;0;FLOAT3;3;FLOAT3;4;FLOAT3;5;FLOAT3;6
Node;AmplifyShaderEditor.CrossProductOpNode;90;-2766.241,-121.1199;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;70;-2577.287,-70.86201;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;69;-2268.469,-24.17117;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2202.092,279.5187;Inherit;False;Property;_randomize;randomize;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;31;-2107.71,120.0017;Inherit;False;Random Range;-1;;3;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;4;-1993.451,-58.21102;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1907.031,153.5305;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;29;-1742.461,46.0318;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-1556.901,-189.2809;Inherit;True;Property;_MainTex;Texture;6;1;[NoScaleOffset];Create;False;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.VertexToFragmentNode;91;-1528.983,26.16465;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;92;-1546.697,195.9639;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-1296.131,-167.2464;Inherit;True;Property;_TextureSample0;Texture Sample 0;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1;-926.1871,591.6495;Inherit;False;Property;_NormalOrient;NormalOrient;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-604.22,640.8062;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexIdVariableNode;5;-606.8448,369.614;Inherit;False;0;1;INT;0
Node;AmplifyShaderEditor.VertexColorNode;6;-598.1531,-448.7143;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;26;-993.85,126.6098;Inherit;False;Random Range;-1;;4;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1016.258,274.1686;Inherit;False;Property;_Fuzz;Fuzz;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-735.8527,498.3518;Inherit;False;Property;_ClayxelSize;ClayxelSize;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-612.7521,164.3003;Inherit;False;Property;_Cutoff;Cutoff;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;-422.2181,-152.0223;Inherit;False;Property;_Emission;Emission;7;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-376.6388,-266.5422;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ConditionalIfNode;27;-644.673,177.6142;Inherit;False;True;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-694.7849,72.86772;Inherit;False;Property;_Smoothness1;Smoothness;5;0;Create;False;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-695.9645,-23.20914;Inherit;False;Property;_Metallic;Metallic;0;0;Create;True;0;0;True;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;100;-320.868,309.0922;Inherit;False;float3 color = float3(0, 0, 0)@$float4 tex = float4(0, 0, 0, 0)@$clayxelVertNormalBlend(vertexId , clayxelSize, normalOrient, tex, color, vertexPosition, vertexNormal)@$v.vertex.w=1@$v.ase_texcoord = tex@$#if defined(SHADERPASS_FORWARD) $v.ase_color = float4(color, 1)@$#endif;7;True;5;False;vertexId;INT;0;In;;Inherit;False;False;vertexPosition;FLOAT3;0,0,0;Out;;Inherit;False;False;vertexNormal;FLOAT3;0,0,0;Out;;Inherit;False;False;clayxelSize;FLOAT;0;In;;Inherit;False;False;normalOrient;FLOAT;0;In;;Inherit;False;clayxelComputeVertexURP;False;False;0;6;0;FLOAT;0;False;1;INT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;3;FLOAT;0;FLOAT3;3;FLOAT3;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;94;174.7608,-24.62549;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;False;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;95;174.7608,-24.62549;Float;False;True;-1;2;;0;2;ClayxelFoliageShaderURP;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;14;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;2;Include;;False;;Native;Include;../../Resources/clayxelSRPUtils.cginc;False;;Custom;;0;0;Standard;14;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Cast Shadows;1;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;Vertex Position,InvertActionOnDeselection;0;0;6;False;True;True;True;True;True;False;;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;96;174.7608,-24.62549;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;False;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;98;174.7608,-24.62549;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;False;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;97;174.7608,-24.62549;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;False;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;99;174.7608,-24.62549;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;True;True;True;True;True;0;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;False;0;0;Standard;0;0
WireConnection;34;1;35;0
WireConnection;90;0;34;3
WireConnection;90;1;34;4
WireConnection;70;0;90;0
WireConnection;69;0;70;0
WireConnection;69;1;70;1
WireConnection;31;1;69;0
WireConnection;49;0;31;0
WireConnection;49;1;50;0
WireConnection;29;0;4;0
WireConnection;29;2;49;0
WireConnection;91;0;29;0
WireConnection;11;0;3;0
WireConnection;11;1;91;0
WireConnection;93;0;1;0
WireConnection;26;1;92;0
WireConnection;26;3;11;4
WireConnection;24;0;6;0
WireConnection;24;1;11;0
WireConnection;27;0;11;4
WireConnection;27;1;9;0
WireConnection;27;2;26;0
WireConnection;100;1;5;0
WireConnection;100;4;2;0
WireConnection;100;5;93;0
WireConnection;95;0;24;0
WireConnection;95;2;12;0
WireConnection;95;3;10;0
WireConnection;95;4;8;0
WireConnection;95;6;27;0
WireConnection;95;7;9;0
WireConnection;95;8;100;3
WireConnection;95;10;100;4
ASEEND*/
//CHKSM=76284B836463B01AE6CB27FDC430FDA7ECAD9D0A