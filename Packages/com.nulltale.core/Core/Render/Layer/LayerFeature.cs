using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace CoreLib.Render
{
    public class LayerFeature : ScriptableRendererFeature
    {
        public  SoCollection<Layer> _layers;
        private List<Pass>          _passes;

        // =======================================================================
        private class Pass : ScriptableRenderPass
        {
            public  LayerFeature      _owner;
            public  Layer             _layer;
            private FilteringSettings _filtering;
            private RenderStateBlock  _override;
            private RenderTarget      _output;

            // =======================================================================
            public void Init()
            {
                renderPassEvent = _layer._event;
                _output         = new RenderTarget().Allocate(_layer.name);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                // allocate resources
                var cmd  = CommandBufferPool.Get(nameof(LayerFeature));
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.colorFormat = RenderTextureFormat.ARGB32;
                if (_layer._scale.Enabled)
                {
                    desc.height = (desc.height * _layer._scale).RoundToInt();
                    desc.width  = (desc.width * _layer._scale).RoundToInt();
                }
                
                _output.Get(cmd, in desc);
                
                if (_layer._scale.Enabled == false && _layer._depth)
                {
#if !UNITY_2022_1_OR_NEWER
                    var depth = renderingData.cameraData.renderer.cameraDepthTarget == BuiltinRenderTextureType.CameraTarget
                        ? renderingData.cameraData.renderer.cameraColorTarget
                        : renderingData.cameraData.renderer.cameraDepthTarget;
#else
                    var depth = renderingData.cameraData.renderer.cameraDepthTargetHandle;
#endif
                    cmd.SetRenderTarget(_output.Id, depth.nameID);
                }
                else
                {
                    cmd.SetRenderTarget(_output.Id);
                }
                
                if (_layer._clear.Enabled)
                {
                    cmd.ClearRenderTarget(RTClearFlags.Color, _layer._clear, 1f, 0);
                }

                try
                {
                    foreach (var rnd in _layer._list)
                        cmd.DrawRenderer(rnd, rnd.sharedMaterial);
                }
                catch
                {
                    // ignored
                }

                _layer._list.Clear();

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                RTHandles.Release(_output);
            }
        }
        
        // =======================================================================
        public override void Create()
        {
            _passes = _layers
                      .Values
                      .Select(n => new Pass() {_owner = this, _layer = n})
                      .ToList();

            foreach (var pass in _passes)
                pass.Init();
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // in game or scene view only
            if (renderingData.cameraData.cameraType != CameraType.Game && renderingData.cameraData.cameraType != CameraType.SceneView)
                return;
            
            foreach (var pass in _passes)
                renderer.EnqueuePass(pass);
        }
    }
}
    