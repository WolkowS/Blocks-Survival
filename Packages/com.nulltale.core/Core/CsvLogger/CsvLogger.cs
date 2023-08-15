using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CoreLib.States;
using CsvHelper;
using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(CsvLogger), menuName = Core.k_CoreModuleMenu + nameof(CsvLogger))]
    public class CsvLogger : Core.Module
    {
        [NaughtyAttributes.ReadOnly(inEditor:false)]
        public bool             _active;
        public Optional<string> _name;
        public GlobalStateBase      _session;
        public GlobalStateBase      _record;
        public string           _path = "Assets\\{0}.csv";
        public Mode             _mode;
        public bool             _postTime = true;
        [NaughtyAttributes.ReadOnly]
        public float _time;
        [NaughtyAttributes.ReadOnly]
        public int _posts;
        [NonSerialized]
        public float _timePrev;
        public float _interval;
        [NonSerialized]
        public bool _postRequest;
        public SoCollection<Module> _mods;

        private StreamWriter _writer;
        private CsvWriter    _csv;
        private Updater      _upd;
        
        private List<Module> _tracks;

        // =======================================================================
        public abstract class Module : ScriptableObject
        {
            public bool _active = true;
            public Optional<string> _name;
            
            // =======================================================================
            public abstract string Value();
            
            public virtual string Name() => _name.GetValueOrDefault(name);

            public virtual void Init(CsvLogger logger) { }
        }

        public class Updater : MonoBehaviour
        {
            public CsvLogger _logger;
            
            // =======================================================================
            private void LateUpdate()
            {
                 _logger._tick();
            }
        }

        [Serializable]
        public enum Mode
        {
            Logs,
            Single
        }

        // =======================================================================
        public override void Init()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += change =>
            {
                if (_session.IsOpen)
                    return;
                
                _csv?.Dispose();
                _writer?.Dispose();
                
                _csv = null;
                _writer = null;
            };
#endif

            _time     = 0f;
            _timePrev = 0f;
            _posts    = 0;
            _tracks   = _mods.Values.Where(n => n._active).ToList();
            
            foreach (var mod in _tracks)
                mod.Init(this);
            
            if (_active == false)
                return;
            
            _upd = Core.Instance.gameObject.AddComponent<Updater>();
            _upd.hideFlags |= HideFlags.NotEditable;
            _upd._logger = this;
            _upd.enabled = false;
            
            _session.OnOpen  += _open;
            _session.OnClose += _close;
            
            if (_session.IsOpen)
                _open();
        }
        
        private void _open()
        {
            _time  = 0;
            _posts = 0;
            var logName = _name.Enabled ? _name.Value : name;
            var fileName = _mode switch
            {
                Mode.Logs   => $"{logName} {DateTime.Now:yy.MM.dd HH.mm.ss}",
                Mode.Single => logName,
                _           => throw new ArgumentOutOfRangeException()
            };
            
            _writer = new StreamWriter(string.Format(_path, fileName));
            _csv    = new CsvWriter(_writer, CultureInfo.InvariantCulture);

            // write header
            if (_postTime)
                _csv.WriteField("Time");
            
            foreach (var mod in _tracks)
                _csv.WriteField(mod.Name());
            
            _upd.enabled = true;
        }
        
        private void _close()
        {
            _upd.enabled = false;
            
            _csv.Dispose();
            _writer.Dispose();
        }

        private void _tick()
        {
            var excess =_time - _timePrev; 
            if (_postRequest || excess >= _interval)
            {
                _postRequest = false;
                _posts ++;
                _timePrev += (excess / _interval).FloorToInt() * _interval;
            
                // do post
                _csv.NextRecord();
                if (_postTime)
                    _csv.WriteField(_time.ToString("F1", CultureInfo.InvariantCulture));
                
                foreach (var mod in _mods)
                    _csv.WriteField(mod.Value());
                
                _csv.Flush();
            }
            
            if (_record.IsOpen)
                _time += Time.deltaTime;
        }
    }
}