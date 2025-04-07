Shader "Hidden/VoxelPostProcess"
{
    Properties
    {
        _VoxelSize ("Voxel Size", Float) = 8.0
        //_Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _VoxelSize;
            
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 voxelUV = floor(i.uv * _ScreenParams.xy / _VoxelSize) * _VoxelSize / _ScreenParams.xy;
                return tex2D(_MainTex, voxelUV);
            }
            
            ENDHLSL
        }
    }
}