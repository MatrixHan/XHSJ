﻿//溶解 控制Startime来溶解
Shader "Seven/Dissolve"   
{  
    Properties   
    {  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
        _NoiseTex ("NoiseTex (R)",2D) = "white"{} 
        _EdgeWidth("EdgeWidth",Range(0,0.5)) = 0.1  
        _EdgeColor("EdgeColor",Color) =  (1,1,1,1) 
        _DissolveThreshold("DissolveThreshold", Range(0,1)) = 0
    }  
    SubShader   
    {  
        Tags { "RenderType"="Opaque" }  
          
        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert_img  
            #pragma fragment frag  
            #include "UnityCG.cginc"  
               
            uniform sampler2D _MainTex;  
            uniform sampler2D _NoiseTex;  
            uniform float _EdgeWidth;  
            uniform float4 _EdgeColor; 
            uniform float _DissolveThreshold; 

            float4 frag(v2f_img i):COLOR  
            {  
                float noiseValue = tex2D(_NoiseTex,i.uv).r;              
                if(noiseValue <= _DissolveThreshold)  
                {  
                    discard;  
                }  
                  
                float4 texColor = tex2D(_MainTex,i.uv);  
                float EdgeFactor = saturate((noiseValue - _DissolveThreshold)/(_EdgeWidth*_DissolveThreshold));  
                float4 BlendColor = texColor * _EdgeColor;  
                                  
                return lerp(texColor,BlendColor,1 - EdgeFactor);  
            }  
              
            ENDCG  
        }  
    }   
      
    FallBack Off    
} 