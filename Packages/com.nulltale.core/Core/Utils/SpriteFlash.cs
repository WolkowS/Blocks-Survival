using System.Collections.Generic;
using System.Linq;
using CoreLib.Tween;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class SpriteFlash : MonoBehaviour
    {
        [ReadOnly(inEditor: false)]
        public GameObject  _root;
        [ReadOnly(inEditor: false)]
        public Material    _material;
        public Vers<Color> _flash;
        [ReadOnly(inEditor: false)]
        public int         _orderOffset = 1;
        public MatProp _property = new MatProp("_Flash");
        public OscillatorBase _alpha;
        
        private List<SpriteRenderer> _list = new List<SpriteRenderer>();
        private bool                 _active;
        private float   _alphaPrev = -1f;
        
        // =======================================================================
        private void Start()
        {
            _active   = true;
            _material = new Material(_material);
            Build();
        }

        private void Update()
        {
            var alpha = _alpha.Value;
            if (_alphaPrev == alpha)
                return;
            
            _alphaPrev = alpha;
            
            _material.SetColor(_property.Hash, _flash.Value.WithA(alpha));

            var isActive = alpha > 0;
            
            if (_active != isActive)
            {
                _active = isActive;
                foreach (var sm in _list)
                    sm.gameObject.SetActive(_active);
            }
        }
        
        public void Clear()
        {
            foreach (var sr in _list.Where(n => n != null))
                DestroyImmediate(sr.gameObject);
            
            _list.Clear();
        }

        public void Build()
        {
            Clear();
            
            var copy = _root.CopyHierarchy((initial, copy) =>
            {
                if (initial.TryGetComponent<SpriteRenderer>(out var sr) == false)
                {
                    copy.AddComponent<TransformLocalMimic>()._target = initial.transform;
                    return;
                }
                    
                var rend = copy.AddComponent<SpriteRenderer>();
                rend.sharedMaterial = _material;

                var sm = copy.AddComponent<SpriteMimic>();
                sm._target      = sr;
                sm._orderOffset = _orderOffset;
                sm._colorMode   = SpriteMimic.ColorMode.Copy;

                var tm = copy.AddComponent<TransformLocalMimic>();
                tm._target = sr.transform;

                _list.Add(rend); 
            });
            
            copy.transform.SetParent(transform);
        }
    }
}