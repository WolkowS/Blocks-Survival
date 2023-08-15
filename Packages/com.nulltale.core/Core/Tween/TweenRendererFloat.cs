using System;
using UnityEngine;

namespace CoreLib.Tween
{
    public class TweenRendererFloat : Tween
    {
        public  string   _property;
        private int      _propertyId;
        private float    _impact;
        private Material _mat;

        // =======================================================================
        private void Awake()
        {
            _propertyId = Shader.PropertyToID(_property);
            _mat        = m_Root.GetValueOrDefault(gameObject).GetComponent<Renderer>().material;  
        }
        
        public override void Apply()
        {
            var impact = m_Input.Value * Weight;
            if (impact == _impact)
                return;
            
            _mat.SetFloat(_propertyId, _mat.GetFloat(_propertyId) + impact - _impact);
            _impact = impact;
        }

        public override void Revert()
        {
            _mat.SetFloat(_propertyId, _mat.GetFloat(_propertyId) - _impact);
        }
    }
}