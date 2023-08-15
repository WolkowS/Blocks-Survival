using System.Linq;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Artistic
{
    public class ArtisticPass : ScriptableRenderPass
    {
        private const  int k_FlipPass       = 0;
        private const  int k_GrayscalePass  = 1;
        private const  int k_GuidelinesPass = 2;
        private static int s_MainTexId      = Shader.PropertyToID("_MainTex");
        
        // =======================================================================
        // https://forum.unity.com/threads/rendertargethandle-is-obsolete-deprecated-in-favor-of-rthandle.1211052/
        // https://github.com/Unity-Technologies/Graphics/blob/3743abbb9d150dc7220ed875dade078136371228/Packages/com.unity.render-pipelines.universal/Documentation~/renderer-features/how-to-fullscreen-blit.md
        //private RTHandle _source;
        private RTHandle _tmp;
        public ArtisticFeature _owner;
        private int      _tmpId;
        
        private        Material _mat;
        private        Mesh     _mesh;
        public         RTHandle _target;

        public bool HasFeatures => _owner._flip || _owner._guidelines || _owner._grayscale;

        // =======================================================================
        public ArtisticPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            
            _tmp    = RTHandles.Alloc(nameof(_tmp), name: nameof(_tmp));
            _tmpId  = Shader.PropertyToID(nameof(_tmp));
            _mat    = new Material(Shader.Find("Hidden/ArtisticFeature"));
            
            _mesh = new Mesh();
            _mesh.SetVertices(new [] 
            {
                new Vector3(-1, _screenPos(1f / 3f), 0), new Vector3(1, _screenPos(1f / 3f), 0),
                new Vector3(-1, _screenPos(2f / 3f), 0), new Vector3(1, _screenPos(2f / 3f), 0),
                new Vector3(_screenPos(1f / 3f), -1, 0), new Vector3(_screenPos(1f / 3f), 1, 0),
                new Vector3(_screenPos(2f / 3f), -1, 0), new Vector3(_screenPos(2f / 3f), 1, 0),
            });
            _mesh.SetIndices(Enumerable.Range(0, _mesh.vertexCount).ToArray(), MeshTopology.Lines, 0);

            // -----------------------------------------------------------------------
            float _screenPos(float ratio)
            {
                return 2f * ratio - 1f;
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd        = CommandBufferPool.Get();
            
            // can't read and write to same color target, create a temp render target to blit.
            cmd.GetTemporaryRT(_tmpId, _target.rt.descriptor, FilterMode.Point);
            
            if (_owner._flip)
                _blit(k_FlipPass);
            
            if (_owner._grayscale)
                _blit(k_GrayscalePass);
            
            if (_owner._guidelines)
            {
                _mat.SetColor("_Color", _owner._color);
                cmd.DrawMesh(_mesh, Matrix4x4.identity, _mat, 0, k_GuidelinesPass);
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);

            // -----------------------------------------------------------------------
            void _blit(int pass)
            {
                cmd.SetGlobalTexture(s_MainTexId, _target);
                cmd.SetRenderTarget(_tmp);
                cmd.DrawMesh(Utils.FullscreenMesh, Matrix4x4.identity, _mat, 0, pass);
                
                cmd.SetGlobalTexture(s_MainTexId, _tmp);
                cmd.SetRenderTarget(_target);
                cmd.DrawMesh(Utils.FullscreenMesh, Matrix4x4.identity, _mat, 0, 3);
            }
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tmpId);
        }
    }
}