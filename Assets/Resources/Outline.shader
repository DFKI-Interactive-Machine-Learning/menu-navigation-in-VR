Shader "Custom/Outline"{
    Properties{
        _Color("Main Color", Color) = (0.5,0.5,0.5,1)
        [HideInInspector]
        _HoverColor("Color Enter",Color) = (0.8301887,0.5456271,0.5456271,1.0)
        [HideInInspector]
        _RenderColor("Visible Color",Color) = (1,1,1,1)

        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(1,5)) = 1
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 pos : POSITION;
        float3 normal : NORMAL;
    };

    float _OutlineWidth;
    float4 _OutlineColor;
    float4 _Color;
    float4 _RenderColor;

    v2f vert(appdata v)
    {
        v.vertex.xyz *= _OutlineWidth;

        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        return o;
    }
    ENDCG

    Subshader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}    
    // Tags{ "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 3000

        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass //Rendering Outlines 
        {
            Zwrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 

            half4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
            }
            ENDCG
        }
        Pass // Normal Render
        {
            ZWrite Off
            Lighting Off


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 

            half4 frag(v2f i) : COLOR
            {

                return _OutlineColor;
            }
            ENDCG

            // Material
            // {
            //     Diffuse[_Color]
            //     Ambient[_Color]
            // }


            // SetTexture[_MainTex]
            // {
            //     ConstantColor[_Color]
            // }

            // SetTexture[_MainTex]
            // {
            //     Combine previous * primary DOUBLE
            // }
        }
    }
}