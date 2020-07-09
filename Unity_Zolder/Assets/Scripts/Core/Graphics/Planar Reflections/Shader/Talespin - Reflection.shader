// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Talespin-Core/Reflection"
{
	Properties
	{
		_SpecColor("Specular Color",Color)=(1,1,1,1)
		[HideInInspector]_ReflectionTexture0("_ReflectionTexture0", 2D) = "white" {}
		[HideInInspector]_ReflectionTexture1("_ReflectionTexture1", 2D) = "white" {}
		_ReflectionIntensity("Reflection Intensity", Range( 0 , 1)) = 0
		_ReflectionBlur("_ReflectionBlur", Range( 0 , 7)) = 3.5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 2.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
		};

		uniform sampler2D _ReflectionTexture0;
		uniform sampler2D _ReflectionTexture1;
		uniform float _ReflectionBlur;
		uniform float _ReflectionIntensity;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float4 ReflectionSampler110( sampler2D ReflectionTex0 , sampler2D ReflectionTex1 , float _ReflectionBlur , float4 Reflection )
		{
			float2 uv = Reflection.xy / Reflection.w; 
			float4 refl0 = tex2Dbias(ReflectionTex0, float4(uv.x, uv.y, 0.0, _ReflectionBlur));
			float4 refl1 = tex2Dbias(ReflectionTex1, float4(uv.x, uv.y, 0.0, _ReflectionBlur));
			float eyeIndex = 0.0;
			if (unity_CameraProjection[0][2] > 0)
				{
					eyeIndex = 1.0;
				}
				else 
				{
					eyeIndex = 0.0;
				}
			return lerp(refl0, refl1, eyeIndex);
		}


		void surf( Input i , inout SurfaceOutput o )
		{
			sampler2D ReflectionTex0110 = _ReflectionTexture0;
			sampler2D ReflectionTex1110 = _ReflectionTexture1;
			float _ReflectionBlur110 = _ReflectionBlur;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 Reflection110 = ase_grabScreenPosNorm;
			float4 localReflectionSampler110 = ReflectionSampler110( ReflectionTex0110 , ReflectionTex1110 , _ReflectionBlur110 , Reflection110 );
			o.Emission = ( localReflectionSampler110 * _ReflectionIntensity ).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17600
1032;478;1956;1167;2336.707;1196.332;1.484605;True;True
Node;AmplifyShaderEditor.RangedFloatNode;152;-1446.706,-299.4198;Float;False;Property;_ReflectionBlur;_ReflectionBlur;4;0;Create;True;0;0;False;0;3.5;3;0;7;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;151;-1413.903,-513.818;Float;True;Property;_ReflectionTexture1;_ReflectionTexture1;2;1;[HideInInspector];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;111;-1420.941,-746.1982;Float;True;Property;_ReflectionTexture0;_ReflectionTexture0;1;1;[HideInInspector];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GrabScreenPosition;113;-1439.404,-153.564;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomExpressionNode;110;-1039.838,-439.6458;Float;False;float2 uv = Reflection.xy / Reflection.w@ $$float4 refl0 = tex2Dbias(ReflectionTex0, float4(uv.x, uv.y, 0.0, _ReflectionBlur))@$float4 refl1 = tex2Dbias(ReflectionTex1, float4(uv.x, uv.y, 0.0, _ReflectionBlur))@$$float eyeIndex = 0.0@$$if (unity_CameraProjection[0][2] > 0)$	{$		eyeIndex = 1.0@$	}$	else $	{$		eyeIndex = 0.0@$	}$$return lerp(refl0, refl1, eyeIndex)@;4;False;4;True;ReflectionTex0;SAMPLER2D;;In;;Float;False;True;ReflectionTex1;SAMPLER2D;;In;;Float;False;True;_ReflectionBlur;FLOAT;2.5;In;;Float;False;True;Reflection;FLOAT4;0,0,0,0;In;;Float;False;Reflection Sampler;True;False;0;4;0;SAMPLER2D;;False;1;SAMPLER2D;;False;2;FLOAT;2.5;False;3;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1052.901,-227.6425;Float;False;Property;_ReflectionIntensity;Reflection Intensity;3;0;Create;True;0;0;False;0;0;0.828;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-691.4564,-446.1867;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-317.4101,-595.8578;Float;False;True;-1;0;ASEMaterialInspector;0;0;BlinnPhong;Talespin-Core/Reflection;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;0;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;110;0;111;0
WireConnection;110;1;151;0
WireConnection;110;2;152;0
WireConnection;110;3;113;0
WireConnection;65;0;110;0
WireConnection;65;1;62;0
WireConnection;0;2;65;0
ASEEND*/
//CHKSM=8B6FBA07430E330056CF74B4691EB3FE1E571973