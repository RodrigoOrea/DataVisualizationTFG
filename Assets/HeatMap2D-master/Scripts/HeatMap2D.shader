Shader "HeatMap2D"
{
	Properties
	{
		[NoScaleOffset] _MainTex("HeatTexture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert          
			#pragma fragment frag

			uniform float _InvRadius = 10.0f;
			uniform float _Intensity = 0.1f;
			uniform int _Count = 0;
			uniform float4 _Points[1023];
			sampler2D _MainTex;

			struct v2f 
			{
				float4 cpos : SV_POSITION;
				float3 wpos : TEXCOORD0;
			};

			v2f vert(float4 vertexPos : POSITION)
			{
				v2f output;
				output.cpos = UnityObjectToClipPos(vertexPos);
				output.wpos = mul(unity_ObjectToWorld, vertexPos);
				return output;
			}
			
			float4 frag(v2f input) : SV_TARGET
			{
				float heat = 0.0f, dist;
				float3 vec;

				for (int i = 0; i < _Count; ++i)
				{
					vec = input.wpos - _Points[i].xyz;
					vec.y = 0.0f; // Asegúrate de que la distancia solo se calcule en el plano XY
					// Calcula la distancia al punto de calor
					// y la intensidad del calor
					// El valor de saturar se calcula como 1 - (distancia / radio)
					// El radio se invierte para que el valor sea más grande cuanto más cerca esté
					// y más pequeño cuanto más lejos esté
					dist = sqrt(dot(vec, vec));
					heat += (1.0f - saturate(dist * _InvRadius)) * _Intensity * _Points[i].w;
				}

				float4 col = tex2D(_MainTex, float2(saturate(heat), 0.5f));

				// Garantiza que no sea completamente invisible
				col.a = max(col.a, 0.05); // puedes ajustar 0.05 a lo que necesites

				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
