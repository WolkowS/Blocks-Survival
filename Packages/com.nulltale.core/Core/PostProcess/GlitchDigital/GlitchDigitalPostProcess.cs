using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render
{
    public class GlitchDigitalPostProcess : PostProcess.Pass
    {
        private static readonly int s_GlitchTex = Shader.PropertyToID("_GlitchTex");
        private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int s_BufferTex = Shader.PropertyToID("_BufferTex");
        
        protected override bool Invert => true;
        
        public Vector2Int    _noiseSize = new Vector2Int(64, 32);
        [Range(0, 1)]
        public float         _noiseUpdateChanse = .85f;
        [Range(0, 1)]
        public float         _pixelUpdateChanse = .85f;

        private RenderTarget _oldFrameA;
        private RenderTarget _oldFrameB;

        private Texture2D _noiseTex;
        private int       _frames;

        // =======================================================================
        public override void Init()
        {
            _oldFrameA = new RenderTarget().Allocate(nameof(_oldFrameA));
            _oldFrameB = new RenderTarget().Allocate(nameof(_oldFrameB));
            
            _updateNoise();
        }

        public override void Cleanup(CommandBuffer cmd)
        {
            _oldFrameA.Release(cmd);
            _oldFrameB.Release(cmd);
        }

        public override void Invoke(CommandBuffer cmd, RTHandle source, RTHandle dest, ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.colorFormat = RenderTextureFormat.ARGB32;
            _oldFrameA.Get(cmd, in desc);
            _oldFrameB.Get(cmd, in desc);

            // Update old frame buffers with the constant interval.
            if ((_frames % 13) == 0) 
                Utils.Blit(cmd, source, _oldFrameA, _material, 1, Invert);
            
            if ((_frames % 73) == 0) 
                Utils.Blit(cmd, source, _oldFrameB, _material, 1, Invert);
            
            cmd.SetGlobalTexture(s_BufferTex, (Random.value > .5f ? _oldFrameA : _oldFrameB).Handle);
            
            Utils.Blit(cmd, source, dest, _material, 0, Invert);
         
            _frames ++;   
        }
        
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<GlitchDigitalSettings>();

            if (settings.IsActive() == false)
            {
                _frames = 0;
                return false;
            }

            if (Random.value > _noiseUpdateChanse.OneMinus())
                _updateNoise();
            
            mat.SetFloat(s_Intensity, settings._intensity.value);
            mat.SetTexture(s_GlitchTex, _noiseTex);
            
            return true;
        }
        
        // =======================================================================
        private void _updateNoise()
        {
            if (_noiseTex == null || _noiseTex.width != _noiseSize.x || _noiseTex.height != _noiseSize.y)
            {
                _noiseTex            = new Texture2D(_noiseSize.x, _noiseSize.y, TextureFormat.ARGB32, false);
                _noiseTex.hideFlags  = HideFlags.DontSave;
                _noiseTex.wrapMode   = TextureWrapMode.Clamp;
                _noiseTex.filterMode = FilterMode.Point;
            }
            
            var color = UnityRandom.Color();

            for (var y = 0; y < _noiseTex.height; y++)
            for (var x = 0; x < _noiseTex.width; x++)
            {
                if (Random.value > _pixelUpdateChanse)
                    color = UnityRandom.Color();
                
                _noiseTex.SetPixel(x, y, color);
            }
            
            _noiseTex.Apply();
        }
    }
}