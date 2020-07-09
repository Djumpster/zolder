Shader "Unlit/Transparent Color"
{
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			Cull Off
			ZWrite Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				float4 color;

				float4 vert( float4 vertex : POSITION ) : SV_POSITION
				{
					return vertex.xyzw;
				}

				float4 frag() : SV_Target
				{
					return color.rgba;
				}
			ENDCG
		}
	}
}