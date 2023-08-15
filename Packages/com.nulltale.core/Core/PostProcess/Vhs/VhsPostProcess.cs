using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace CoreLib.Render
{
    public class VhsPostProcess : PostProcess.Pass
    {
        private static readonly int s_VHSTex    = Shader.PropertyToID("_VHSTex");
        private static readonly int s_XScanline = Shader.PropertyToID("_xScanline");
        private static readonly int s_YScanline = Shader.PropertyToID("_yScanline");
        private static readonly int s_Rocking   = Shader.PropertyToID("_Rocking");

        public                  VideoClip   _clip;
        private                 VideoPlayer _player;
		private                 float       _yScanline;
		private                 float       _xScanline;

        protected override bool Invert => true;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<VhsSettings>();

            if (_player == null)
            {
                _player = Core.Instance.gameObject.AddComponent<VideoPlayer>();
                _player.hideFlags = HideFlags.HideAndDontSave;
                _player.isLooping = true;
                _player.renderMode = VideoRenderMode.APIOnly;
                _player.audioOutputMode = VideoAudioOutputMode.None;
                _player.clip = _clip;
            }

            var isActive = settings.IsActive();
            
            if (isActive)
                _player.Play();
            else
                _player.Pause();
            
            if (isActive == false)
                return false;

            // scale line
			_yScanline += Time.deltaTime * 0.01f * settings._bleed.value;
			_xScanline -= Time.deltaTime * 0.1f * settings._bleed.value;
            
			if (_yScanline >= 1)
				_yScanline = Random.value;
            
			if (_xScanline <= 0 || Random.value < 0.05)
				_xScanline = Random.value;
            
			mat.SetFloat(s_YScanline, _yScanline);
			mat.SetFloat(s_XScanline, _xScanline);
			mat.SetFloat(s_Rocking, settings._rocking.value);
            
            // params
			mat.SetTexture(s_VHSTex, _player.texture);
            
            return true;
        }
    }
}