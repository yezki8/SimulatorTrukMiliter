Shader "Unlit/FogOfWarStandard"
{
    Properties
    {
        _MainTex ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed4 _FogColor;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = _FogColor.rgb;
            o.Alpha = _FogColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
