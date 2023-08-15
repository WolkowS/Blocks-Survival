using System;
using CoreLib;
using CoreLib.Render;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CoreLib.Render.RenderFeature
{
	public class Blit : ScriptableRendererFeature 
	{
		public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;

		public Material                     _material;
		public Optional<BlitPassController> _controller;

		public Optional<string> _mainTex = new Optional<string>("_MainTex", false);
		public Optional<int>    _pass    = new Optional<int>(0, false);

		public TextureSource    _srcTex = new TextureSource();
		public TextureSource    _dstTex = new TextureSource();

		public  bool           _overrideFormat;
		[ShowIf(nameof(_overrideFormat))]
		public  GraphicsFormat _graphicsFormat;
		
		private BlitPass       _blitPass;
		
		// =======================================================================
		public class BlitPass : ScriptableRenderPass 
		{
			public Material		 _material;
			private Blit		 _owner;

			private RTHandle           Source         { get; set; }
			private RTHandle           Destination    { get; set; }
			private BlitPassController _passController;

			private RenderTarget  _tmpTex;
			private RTHandle  _desTex;
			private string    _profilerTag;
			private int		  _mainTexId;

			private bool     _execute;
			private Material _common;

			// =======================================================================
			public BlitPass(RenderPassEvent passEvent, Blit owner, string tag, BlitPassController passController) 
			{
				renderPassEvent = passEvent;
				
				_passController = passController;
				_owner          = owner;
				_material       = owner._material;
				_profilerTag    = tag;
				_mainTexId      = Shader.PropertyToID(owner._mainTex.GetValueOrDefault("_MainTex"));
				_common         = new Material(Shader.Find("Hidden/Common"));
				_tmpTex         = new RenderTarget().Allocate(nameof(_tmpTex));
				
				if (owner._dstTex._type == Target.TextureId)
					_desTex = RTHandles.Alloc(owner._dstTex._id, name: owner._dstTex._id);
			}

			public bool Setup(ScriptableRenderer renderer, ref RenderingData renderingData) 
			{
				if (_passController != null)
                {
					_passController.Validate(_material);
					if (_passController.IsActive)
					    _execute = true;
                }
				else
				{
					_execute = true;
				}

				if (_execute == false)
					return false;
				
                return true;
			}

			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) 
			{
				var cmd = CommandBufferPool.Get(_profilerTag);

				var renderer   = renderingData.cameraData.renderer;
				var opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
				
				Source = _owner._srcTex._type switch
				{
					Target.CameraColor  => renderer.cameraColorTargetHandle,
					Target.TextureId    => RTHandles.Alloc(new RenderTargetIdentifier(_owner._srcTex._id)),
					Target.TextureAsset => RTHandles.Alloc(new RenderTargetIdentifier(_owner._srcTex._asset)),
					_                   => throw new ArgumentOutOfRangeException()
				};

				Destination = _owner._dstTex._type switch
				{
					Target.CameraColor  => renderer.cameraColorTargetHandle,
					Target.TextureId    => RTHandles.Alloc(new RenderTargetIdentifier(_owner._dstTex._id)),
					Target.TextureAsset => RTHandles.Alloc(new RenderTargetIdentifier(_owner._dstTex._asset)),
					_                   => throw new ArgumentOutOfRangeException()
				};

				if (_owner._dstTex._type == Target.TextureId)
				{
					if (_owner._overrideFormat)
						opaqueDesc.graphicsFormat = _owner._graphicsFormat;

					cmd.GetTemporaryRT(Shader.PropertyToID(_desTex.name), opaqueDesc);
				}

				if (_isSameTexture())
				{
					_tmpTex.Get(cmd, in opaqueDesc);
					
	                cmd.SetGlobalTexture(_mainTexId, Source.nameID);
	                cmd.SetRenderTarget(_tmpTex.Handle.nameID);
	                cmd.DrawMesh(Utils.FullscreenMesh, Matrix4x4.identity, _material, 0, _owner._pass.GetValueOrDefault(0));
	                
	                cmd.SetGlobalTexture(_mainTexId, _tmpTex.Handle.nameID);
	                cmd.SetRenderTarget(Source.nameID);
	                cmd.DrawMesh(Utils.FullscreenMesh, Matrix4x4.identity, _common, 0, 0);
				}
				else
				{
					Blit(cmd, Source, Destination, _material, _owner._pass.GetValueOrDefault(0));
				}

				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();
				CommandBufferPool.Release(cmd);
			}

			public override void FrameCleanup(CommandBuffer cmd)
			{
				_execute = false;
				
				if (_owner._dstTex._type == Target.TextureId)
					_desTex.Release();

				if (_isSameTexture())
					_tmpTex.Release(cmd);
			}

			private bool _isSameTexture()
			{
				return Source == Destination || (_owner._srcTex == _owner._dstTex && _owner._srcTex._type == Target.CameraColor);
			}
		}

		[Serializable]
		public class TextureSource
		{
			public Target        _type  = Target.CameraColor;
			public string        _id = nameof(_id);
			public RenderTexture _asset;
		}
		
		public enum Target 
		{
			CameraColor,
			TextureId,
			TextureAsset
		}
		
		public abstract class BlitPassController : ScriptableObject
	    {
			public abstract bool IsActive { get; }

	        public abstract void Validate(Material mat);
	    }
		
		// =======================================================================
		public override void Create() 
		{
			var passIndex = _material != null ? _material.passCount - 1 : 1;
			
			_pass.Value = Mathf.Clamp(_pass.GetValueOrDefault(0), 0, passIndex);
			_blitPass   = new BlitPass(_event, this, name, _controller.GetValueOrDefault(null));

			if (_graphicsFormat == GraphicsFormat.None) 
				_graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR);
		}
		
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                _blitPass.ConfigureInput(ScriptableRenderPassInput.Color);
            }
        }

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) 
		{
			if (_material == null) 
				return;

			if (_blitPass.Setup(renderer, ref renderingData))
			    renderer.EnqueuePass(_blitPass);
		}
	}
}