// Simplified Additive Particle shader. Differences from regular Additive Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Game4f-Mobile/Particles Additive" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha ONE
	//Blend SrcAlpha One
	//Blend ONE One
	ColorMask RGB
	Cull Off 
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			SetTexture [_MainTex] {
	        	constantColor [_Color]
	        	combine constant* primary
	        	}
		
			SetTexture [_MainTex] {
				combine texture * Previous double
			}
		}
	}
}
}