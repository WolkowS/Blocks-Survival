using System.Collections.Generic;
using CoreLib.Values;
using TMPro;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(Module.Fps), menuName = Core.k_CoreModuleMenu + nameof(Module.Fps))]
    public class Fps : Core.Module<Fps>
    {
        [Range(0.1f, 60.0f)] public float _observation = 1.0f;
        [SerializeField] private string   _format      = "##.00";
        [SerializeField] private bool     _vSyncOff;
        [SerializeField] private bool     _show;
        [SerializeField] private Rect     _guiRect = new Rect(16, 8, 50, 20);

        private float        _fps;
        private Queue<float> _frames = new Queue<float>();
        private float        _deltaSum;
        
        public float  FpsValue  => _fps;
        public string FpsString => _fps.ToString(_format, Core.k_NumberFormat);

        public bool Show
        {
            get => _show;
            set
            {
                _show = value;
                OnValidate();
            }
        }
        

        private OnGUICallback ShowCallback;

        // =======================================================================
        public override void Init()
        {
            // update callback
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = _update;
            _frames.Clear();
            _deltaSum = 0;
            
            if (_vSyncOff)
            {
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
            }
            
            
            // show callback
            if (_show)
            {
                ShowCallback = Core.Instance.gameObject.AddComponent<OnGUICallback>();
                ShowCallback.Action = _onGuiCallback;
            }
        }
        
        public void Set(TMP_Text text)
        {
            text.text = FpsString;
        }
        
        public void Set(GvFloat value)
        {
            value.Value = FpsValue;
        }
		
        public void _update()
        {
            _frames.Enqueue(Time.deltaTime);
            _deltaSum += Time.deltaTime;
            
            while (_deltaSum > _observation)
                _deltaSum -= _frames.Dequeue();

            _fps = _frames.Count / _deltaSum;
        }

        public void OnValidate()
        {
            // show if required
            if (_show && ShowCallback == null && Application.isPlaying)
            {
                ShowCallback = Core.Instance.gameObject.AddComponent<OnGUICallback>();
                ShowCallback.Action = _onGuiCallback;
            }
            else if (ShowCallback != null)
            {
                Destroy(ShowCallback);
            }
        }

        private void _onGuiCallback()
        {
            if (Event.current.type == EventType.Repaint)
                GUI.Label(_guiRect, FpsString);
        }
    }
}