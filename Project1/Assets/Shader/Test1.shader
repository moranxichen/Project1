// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader"Test/test1"{
	Properties{
		_MainColor("MainColor",color)=(1,1,1,1)
	}
	SubShader {
		Tags { "LightMode"="ForwardBase" }
		Pass {

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment farg
		#include "Lighting.cginc"

		fixed4 _MainColor;

		
		struct a2v{
			float4 vertex :POSITION ;
			float3 normal:NORMAL;
		} ;

		struct v2f{
			float4 pos:SV_POSITION;
			float3 worldNormal : TEXCOORD0 ;

		} ;

		v2f vert (a2v v){
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex );
			o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
			return o;
		}

		fixed4 farg(v2f i):SV_Target{
			fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz ;
			fixed3 worldNormal =normalize(i.worldNormal );
			float3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
			float3 lbt=saturate(dot(worldNormal ,worldLight));
			float3 diffuse=_LightColor0.rgb*_MainColor.rgb*lbt;//*struct(dot(worldNormal ,worldLight));
			fixed3 color=ambient+diffuse;
			return fixed4(color,1);
		}

		ENDCG

		}
	}
	FallBack  "diffuse"
}