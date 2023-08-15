using System;
using CoreLib.Render.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace CoreLib.Render
{
    public class ColorSwapPass : PostProcess.Pass
    {
        private static readonly int s_LutTableTex = Shader.PropertyToID("_LutTableTex");
        private static readonly int s_Weight      = Shader.PropertyToID("_Weight");
        private static readonly int s_Eval        = Shader.PropertyToID("_Eval");
        private static readonly int s_Grades      = Shader.PropertyToID("_Grades");

        public Texture2D _lutDefault;
        private int _lutTableWidth;
        private int _lutSize;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = VolumeManager.instance.stack.GetComponent<ColorSwapSettings>();

            if (settings.IsActive() == false)
                return false;
            
            var lootTable = settings.m_LutTable.overrideState ? settings.m_LutTable.value : _lutDefault;
            _validateLutSize(lootTable.width);
            
            var grades = (float)lootTable.height / _lutSize;
            var sharp  = settings.m_Sharp.value;
            
            mat.SetTexture(s_LutTableTex, lootTable);
            mat.SetFloat(s_Weight, settings.m_Weight.value);
            
            mat.SetFloat(s_Eval, sharp ? 1f - ((settings.m_Eval.value).Round() / (grades)).Clamp01() : 1f - (settings.m_Eval.value / (grades)).Clamp01());
            mat.SetFloat(s_Grades, grades);
            
            return true;
        }
        
        // =======================================================================
        private void _validateLutSize(int width)
        {
            if (_lutTableWidth == width)
                return;
            
            _lutTableWidth = width;
            
            
            _material.DisableKeyword("_LUT_SIZE_X16");
            _material.DisableKeyword("_LUT_SIZE_X32");
            _material.DisableKeyword("_LUT_SIZE_X64");
            
            switch (width)
            {
                case 16 * 16:
                    _material.EnableKeyword("_LUT_SIZE_X16");
                    _lutSize = 16;
                    break;
                case 32 * 32:
                    _material.EnableKeyword("_LUT_SIZE_X32");
                    _lutSize = 32;
                    break;
                case 64 * 64:
                    _material.EnableKeyword("_LUT_SIZE_X64");
                    _lutSize = 64;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unknown loot size");
            }
        }
    }
}