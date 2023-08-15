using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Artistic
{
    public class ArtisticFeature : ScriptableRendererFeature
    {
        public static HashSet<ArtisticFeature> Instances = new HashSet<ArtisticFeature>();
        
        [Tooltip("Flip view horizontal (Shift + F)")]
        public        bool            _flip;
        [Tooltip("Toggle Composition Guide Lines (Shift + G)")]
        public        bool            _guidelines;
        [Tooltip("Toggle grayscale (Shift + J)")]
        public        bool            _grayscale;
        public        Color           _color = new Color(1f, 1f, 1f, 1f);
        private ArtisticPass _pass;

        // =======================================================================
        public override void Create()
        {
#if UNITY_EDITOR
            _pass = new ArtisticPass() { _owner = this };

            Instances.Add(this);
#endif
        }
        
        public void ToggleComposition()
        {
            _guidelines = !_guidelines;
            _validate();
        }
        
        public void ToggleFlip()
        {
            _flip = !_flip;
            _validate();
        }
        
        public void ToggleGrayscale()
        {
            _grayscale = !_grayscale;
            _validate();
        }
        
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
                // ensures that the opaque texture is available to the Render Pass.
                _pass.ConfigureInput(ScriptableRenderPassInput.Color);
                _pass._target = renderer.cameraColorTargetHandle;
            }
#endif
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;
            
            if (_pass.HasFeatures == false)
                return;
            
            renderer.EnqueuePass(_pass);
#endif
        }
        
        private void _validate()
        {
#if UNITY_EDITOR
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
#endif
        }
    }
}