using UnityEngine;

namespace CoreLib.FxTexture
{
    public class Link : FxTextureAsset.PassCombine
    {
        public FxTextureAsset _fxTexture;
        
        // =======================================================================
        public override Color[] _proceed(Color[] input, int width, int height)
        {
            if (_fxTexture == null)
                return input;
            
            var size = new Vector2Int(width, height);
            if (_fxTexture._size != size)
                _fxTexture._size = size;
            
            return _fxTexture.GetPixels();
        }
    }
}