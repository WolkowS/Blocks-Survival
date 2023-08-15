using UnityEngine;

namespace Game.FX
{
    [ExecuteAlways]
    public class SetGlobalTexture : MonoBehaviour
    {
        public string  _name;
        public Texture _texture;

        // =======================================================================
        private void Start()
        {
            Shader.SetGlobalTexture(_name, _texture);
        }
    }
}