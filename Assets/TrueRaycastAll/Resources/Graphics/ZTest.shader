Shader "ZTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_OverlayTex ("Overlay (RGB) Trans (A)", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Geometry+300"}
		Blend SrcAlpha OneMinusSrcAlpha
		pass {
			ZTest LESS 
			ZWrite Off
			Material
			{
				Diffuse[_Color]
			}
			SetTexture[_MainTex]{combine texture * primary}
		}
		pass {
			ZTest LESS 
			ZWrite Off
			Lighting On
			Material 
			{
				Diffuse[_Color]
			}
			SetTexture[_OverlayTex]{combine texture*primary}
		}
	}
}