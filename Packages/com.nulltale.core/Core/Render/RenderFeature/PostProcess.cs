using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render.RenderFeature
{
    public class PostProcess : ScriptableRendererFeature
    {
        public RenderPassEvent    _event  = RenderPassEvent.BeforeRenderingPostProcessing;
        public Optional<string>   _target = new Optional<string>("_SourceTex", false);
        public SoCollection<Pass> _passes;

        [HideInInspector]
        public Shader _blitShader;
        private Material _blit;
        
        private PassExecution   _execution;

        // =======================================================================
        [Serializable]
        public abstract class Pass : ScriptableObject
        {
            public  bool     _active = true;
            public  Material _material;
            private bool     _isActive;
            
            protected virtual bool Invert => false;

            // =======================================================================
            internal bool IsActive
            {
                get => _isActive && _active;
                set => _isActive = value;
            }

            public virtual void Init()
            {
            }

            public virtual void Invoke(CommandBuffer cmd, RTHandle source, RTHandle dest, ScriptableRenderContext context, ref RenderingData renderingData)
            {
                Utils.Blit(cmd, source, dest, _material, 0, Invert);
            }

            public void Validate()
            {
                IsActive = Validate(_material);
            }
            
            public abstract bool Validate(Material mat);
            
            public virtual void Cleanup(CommandBuffer cmd)
            {
            }
        }

        private class PassExecution : ScriptableRenderPass
        {
            public  PostProcess      _owner;
            private RenderTargetFlip _tmp;
            private Pass[]           _passes;

            // =======================================================================
            public void Init()
            {
                renderPassEvent = _owner._event;
                
                _tmp = new RenderTargetFlip(nameof(_tmp));
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                foreach (var pass in _owner._passes.Values)
                    pass.Validate();
                
                _passes = _owner._passes.Values.Where(n => n.IsActive).ToArray();
                if (_passes.Length == 0)
                    return;
                
                // allocate stuff
                var cmd = CommandBufferPool.Get("Post Process");
                ref var cameraData = ref renderingData.cameraData;
                _tmp.Get(cmd, in cameraData.cameraTargetDescriptor);
                
                var target = _owner._target.Enabled ? RTHandles.Alloc(_owner._target.Value) : cameraData.renderer.cameraColorTargetHandle;

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
                // draw post process chain
                if (_passes.Length == 1)
                {
                    Utils.Blit(cmd, target, _tmp.From, _owner._blit);
                    _passes[0].Invoke(cmd, _tmp.From, target, context, ref renderingData);
                }
                else
                {
                    _passes[0].Invoke(cmd, target, _tmp.From, context, ref renderingData);
                    for (var n = 1; n < _passes.Length - 1; n++)
                    {
                        var pass = _passes[n];
                        pass.Invoke(cmd, _tmp.From, _tmp.To, context, ref renderingData);
                        _tmp.Flip();
                    }
                    
                    _passes[_passes.Length - 1].Invoke(cmd, _tmp.From, target, context, ref renderingData);
                }
                
                cmd.SetViewProjectionMatrices(cameraData.GetViewMatrix(), cameraData.GetProjectionMatrix());
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }
            
            public override void FrameCleanup(CommandBuffer cmd)
            {
                _tmp.Release(cmd);
                foreach (var pass in _passes)
                    pass.Cleanup(cmd);
            }
        }

        // =======================================================================
        public override void Create()
        {
#if UNITY_EDITOR
            _blitShader = Shader.Find("Hidden/PostProcess/Blit");
            
            UnityEditor.EditorUtility.SetDirty(this);
#endif
            _blit      = new Material(_blitShader);
            _execution = new PassExecution() { _owner = this};
            _execution.Init();
            
            foreach (var pass in _passes)
            {
#if UNITY_EDITOR
                if (pass == null)
                {
                    Debug.LogWarning("PP Pass is null", pass);
                    continue;
                }
#endif
                pass.Init();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;
            
            renderer.EnqueuePass(_execution);
        }
    }
}