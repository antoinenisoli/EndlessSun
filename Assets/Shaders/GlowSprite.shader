// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Deeper/GlowSprite"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[SingleLineTexture]_Emissive("Emissive", 2D) = "black" {}
		[HDR]_EmissiveMapColor("EmissiveMapColor", Color) = (0,0.7249274,1,1)
		[HDR]_OutlineColor("OutlineColor", Color) = (0,0.8065434,1,1)
		_OutlineWidth("OutlineWidth", Range( 0 , 3)) = 1
		[Toggle(_PULSEANIM_ON)] _PulseAnim("PulseAnim", Float) = 0
		_PulseSpeed("PulseSpeed", Float) = 8
		[HideInInspector]_Emission("Emission", Range( 0 , 1)) = 0
		[HDR]_RimColor("RimColor", Color) = (0,0,0,1)
		_RimThicknessX("RimThicknessX", Range( -0.03 , 0.03)) = 0
		_RimThicknessY("RimThicknessY", Range( -0.03 , 0.03)) = 0
		[HDR]_GlobalGlow("GlobalGlow", Color) = (0,0,0,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _PULSEANIM_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Tint;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;
		uniform float _PulseSpeed;
		uniform float _RimThicknessX;
		uniform float _RimThicknessY;
		uniform float4 _RimColor;
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform float4 _EmissiveMapColor;
		uniform float _Emission;
		uniform float4 _GlobalGlow;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode25 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _Tint * tex2DNode25 ).rgb;
			float textureOpacity66 = tex2DNode25.a;
			float2 uv_TexCoord123 = i.uv_texcoord + ( float2( 0.01,0 ) * _OutlineWidth );
			float2 uv_TexCoord128 = i.uv_texcoord + ( float2( -0.01,0 ) * _OutlineWidth );
			float2 uv_TexCoord139 = i.uv_texcoord + ( _OutlineWidth * float2( 0,0.01 ) );
			float2 uv_TexCoord144 = i.uv_texcoord + ( _OutlineWidth * float2( 0,-0.01 ) );
			float OutlineAlpha179 = ( ( textureOpacity66 - tex2D( _MainTex, uv_TexCoord123 ).a ) + ( textureOpacity66 - tex2D( _MainTex, uv_TexCoord128 ).a ) + ( textureOpacity66 - tex2D( _MainTex, uv_TexCoord139 ).a ) + ( textureOpacity66 - tex2D( _MainTex, uv_TexCoord144 ).a ) );
			float4 temp_output_186_0 = ( OutlineAlpha179 * _OutlineColor );
			float clampResult187 = clamp( cos( ( _Time.y * _PulseSpeed ) ) , 0.1 , 1.0 );
			#ifdef _PULSEANIM_ON
				float4 staticSwitch189 = ( clampResult187 * temp_output_186_0 );
			#else
				float4 staticSwitch189 = temp_output_186_0;
			#endif
			float4 OutlineEmission190 = staticSwitch189;
			float2 appendResult115 = (float2(_RimThicknessX , _RimThicknessY));
			float2 uv_TexCoord103 = i.uv_texcoord + appendResult115;
			float4 rimLight105 = ( saturate( ( textureOpacity66 - tex2D( _MainTex, uv_TexCoord103 ).a ) ) * _RimColor );
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			float4 color94 = IsGammaSpace() ? float4(1,0,0,1) : float4(1,0,0,1);
			float4 lerpResult96 = lerp( ( OutlineEmission190 + rimLight105 + ( tex2D( _Emissive, uv_Emissive ) * _EmissiveMapColor ) ) , color94 , _Emission);
			float4 EmissiveMap90 = ( lerpResult96 + _GlobalGlow );
			o.Emission = EmissiveMap90.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			clip( textureOpacity66 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
2028;89;1742;800;1300.12;402.8185;1.067339;True;True
Node;AmplifyShaderEditor.CommentaryNode;149;-1992.631,1566.806;Inherit;False;5044.586;2537.772;;2;192;194;Procedural Glow;0,0.8575506,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;192;-1869.975,2080.427;Inherit;False;2706.172;1340.912;Comment;25;179;125;146;141;126;118;136;140;124;145;127;144;123;139;128;148;134;129;143;138;132;133;142;137;81;Outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1835.648,2683.285;Inherit;False;Property;_OutlineWidth;OutlineWidth;8;0;Create;True;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;137;-1490.819,2911.282;Inherit;False;Constant;_Vector2;Vector 2;14;0;Create;True;0;0;False;0;False;0,0.01;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;142;-1655.859,3181.937;Inherit;False;Constant;_Vector3;Vector 3;14;0;Create;True;0;0;False;0;False;0,-0.01;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;133;-1507.297,2556.212;Inherit;False;Constant;_Vector1;Vector 1;14;0;Create;True;0;0;False;0;False;-0.01,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;132;-1681.055,2261.654;Inherit;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;False;0;False;0.01,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;101;1001.646,-92.6884;Inherit;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;1363.898,115.5484;Inherit;False;mainTexture;-1;True;1;0;SAMPLER2D;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;-1307.474,2623.254;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-1433.198,2314.879;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-1427.872,3102.686;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;138;-1290.182,2838.171;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;25;1309.45,-100.7066;Inherit;True;Property;_TextureSample;TextureSample;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;148;-1066.665,2223.068;Inherit;False;147;mainTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;-1127.917,2340.888;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0.01,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;139;-1096.476,2820.599;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.01,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;144;-1128.207,3076.486;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.01,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;128;-1113.768,2605.682;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.01,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;1699.819,7.282501;Inherit;False;textureOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;140;-737.1014,2821.676;Inherit;True;Property;_TextureSample3;Texture Sample 3;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;136;-663.9602,2154.764;Inherit;False;66;textureOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;124;-738.4618,2313.359;Inherit;True;Property;_TextureSample1;Texture Sample 1;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;145;-727.9939,3121.019;Inherit;True;Property;_TextureSample4;Texture Sample 4;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;127;-742.4377,2566.782;Inherit;True;Property;_TextureSample2;Texture Sample 2;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;152;-1810.177,643.3937;Inherit;False;2394.112;579.8782;Comment;12;116;114;115;103;102;99;113;107;112;105;150;151;RimLight;1,0.9371229,0,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;126;-244.9509,2560.537;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;118;-242.5329,2249.394;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-1734.1,921.3264;Inherit;False;Property;_RimThicknessX;RimThicknessX;13;0;Create;True;0;0;False;0;False;0;0.01;-0.03;0.03;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;141;-228.5616,2878.877;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;146;-235.0436,3192.276;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-1760.177,1054.551;Inherit;False;Property;_RimThicknessY;RimThicknessY;14;0;Create;True;0;0;False;0;False;0;0;-0.03;0.03;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;194;1194.185,2116.607;Inherit;False;1698.234;672.6731;;11;189;190;183;193;188;187;186;184;182;181;180;Pulse;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;181;1252.75,2166.608;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;125;197.3971,2747.731;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;180;1251.912,2257.262;Inherit;False;Property;_PulseSpeed;PulseSpeed;10;0;Create;True;0;0;False;0;False;8;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;115;-1368.19,969.7868;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;103;-1153.691,920.358;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;-0.02,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;151;-1048.807,770.5524;Inherit;False;147;mainTexture;1;0;OBJECT;;False;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;1444.298,2192.055;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;6.62;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;179;498.379,2814.45;Inherit;False;OutlineAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;-757.335,693.3937;Inherit;False;66;textureOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CosOpNode;184;1596.583,2245.959;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;193;1256.675,2457.982;Inherit;False;179;OutlineAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;102;-775.5706,820.0826;Inherit;True;Property;_TextureSample0;Texture Sample 0;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;183;1244.185,2580.281;Inherit;False;Property;_OutlineColor;OutlineColor;7;1;[HDR];Create;True;0;0;False;0;False;0,0.8065434,1,1;0,0.465055,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;1525.078,2503.412;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;8.7,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;187;1763.879,2231.955;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;99;-367.9845,774.6105;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;107;-121.6344,773.9869;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;113;-160.6883,1014.272;Inherit;False;Property;_RimColor;RimColor;12;1;[HDR];Create;True;0;0;False;0;False;0,0,0,1;1,0.5928419,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;1972.14,2268.755;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;153;-1795.85,-445.7237;Inherit;False;1695.481;723.3339;Comment;11;42;41;106;80;43;79;94;98;96;196;197;Map Emissive;0.06649756,1,0,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;189;2333.535,2520.562;Inherit;False;Property;_PulseAnim;PulseAnim;9;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;112;133.0859,873.4686;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;42;-1685.657,-260.2902;Inherit;True;Property;_Emissive;Emissive;5;1;[SingleLineTexture];Create;True;0;0;False;0;False;-1;0000000000000000f000000000000000;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;190;2655.618,2583.699;Inherit;False;OutlineEmission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;41;-1644.189,-38.88876;Inherit;False;Property;_EmissiveMapColor;EmissiveMapColor;6;1;[HDR];Create;True;0;0;False;0;False;0,0.7249274,1,1;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;359.1346,781.304;Inherit;False;rimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;80;-1256.359,-389.3766;Inherit;False;190;OutlineEmission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1271.744,-137.8207;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.5,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-1242.399,-307.0162;Inherit;False;105;rimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;94;-966.5297,-36.32209;Inherit;False;Constant;_HitColor;HitColor;10;0;Create;True;0;0;False;0;False;1,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;98;-995.6129,162.2108;Float;False;Property;_Emission;Emission;11;1;[HideInInspector];Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;-971.2678,-283.3806;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;96;-666.9135,-118.3502;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;196;-648.7786,113.6962;Inherit;False;Property;_GlobalGlow;GlobalGlow;15;1;[HDR];Create;True;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;197;-332.0428,-22.84575;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;89;1357.976,-313.4999;Inherit;False;Property;_Tint;Tint;2;0;Create;True;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-33.03647,0.162272;Inherit;False;EmissiveMap;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;39;1999.466,8.61776;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;1707.547,-279.847;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;2035.084,-94.60975;Inherit;False;90;EmissiveMap;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;2059.212,195.9063;Inherit;False;66;textureOpacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;1987.107,98.50976;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;24;2377.233,-171.1039;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Deeper/GlowSprite;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;147;0;101;0
WireConnection;134;0;133;0
WireConnection;134;1;81;0
WireConnection;129;0;132;0
WireConnection;129;1;81;0
WireConnection;143;0;81;0
WireConnection;143;1;142;0
WireConnection;138;0;81;0
WireConnection;138;1;137;0
WireConnection;25;0;101;0
WireConnection;123;1;129;0
WireConnection;139;1;138;0
WireConnection;144;1;143;0
WireConnection;128;1;134;0
WireConnection;66;0;25;4
WireConnection;140;0;148;0
WireConnection;140;1;139;0
WireConnection;124;0;148;0
WireConnection;124;1;123;0
WireConnection;145;0;148;0
WireConnection;145;1;144;0
WireConnection;127;0;148;0
WireConnection;127;1;128;0
WireConnection;126;0;136;0
WireConnection;126;1;127;4
WireConnection;118;0;136;0
WireConnection;118;1;124;4
WireConnection;141;0;136;0
WireConnection;141;1;140;4
WireConnection;146;0;136;0
WireConnection;146;1;145;4
WireConnection;125;0;118;0
WireConnection;125;1;126;0
WireConnection;125;2;141;0
WireConnection;125;3;146;0
WireConnection;115;0;114;0
WireConnection;115;1;116;0
WireConnection;103;1;115;0
WireConnection;182;0;181;0
WireConnection;182;1;180;0
WireConnection;179;0;125;0
WireConnection;184;0;182;0
WireConnection;102;0;151;0
WireConnection;102;1;103;0
WireConnection;186;0;193;0
WireConnection;186;1;183;0
WireConnection;187;0;184;0
WireConnection;99;0;150;0
WireConnection;99;1;102;4
WireConnection;107;0;99;0
WireConnection;188;0;187;0
WireConnection;188;1;186;0
WireConnection;189;1;186;0
WireConnection;189;0;188;0
WireConnection;112;0;107;0
WireConnection;112;1;113;0
WireConnection;190;0;189;0
WireConnection;105;0;112;0
WireConnection;43;0;42;0
WireConnection;43;1;41;0
WireConnection;79;0;80;0
WireConnection;79;1;106;0
WireConnection;79;2;43;0
WireConnection;96;0;79;0
WireConnection;96;1;94;0
WireConnection;96;2;98;0
WireConnection;197;0;96;0
WireConnection;197;1;196;0
WireConnection;90;0;197;0
WireConnection;88;0;89;0
WireConnection;88;1;25;0
WireConnection;24;0;88;0
WireConnection;24;2;91;0
WireConnection;24;3;39;0
WireConnection;24;4;40;0
WireConnection;24;10;92;0
ASEEND*/
//CHKSM=23927375CACF7C397449C5C0EDE6CCB53078925A