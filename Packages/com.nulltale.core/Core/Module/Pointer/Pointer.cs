using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Values;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(Pointer), menuName = Core.k_CoreModuleMenu + nameof(Pointer))]
    public class Pointer : Core.Module<Pointer>
    {
        public static readonly Plane		k_GroundPlaneXY = new Plane(Vector3.forward, 0.0f);
        public static readonly Plane		k_GroundPlaneXZ = new Plane(Vector3.up, 0.0f);
        
        internal Texture2D  m_Clean;
        
        [SerializeField]
        private Core.ProjectSpace       m_ProjectSpace;

        public static Plane      GroundPlane    { get; private set; }
        public static Vector2    Screen         { get; private set; }
        public static Vector3    World          { get; private set; }
        public static Vector2Int Cell           { get; private set; }
        public static Vector2    ScreenUV       { get; private set; }
        public static Vector2    ScreenUVAspect { get; private set; }

        private Ref<Vector3>     m_WorldPositionRef = new Ref<Vector3>();
        public  IRefGet<Vector3> WorldPositionRef => m_WorldPositionRef;

        public Ray                 CameraRay;

        private InputAction        m_PointerPosition;

        public Optional<GvVec3>        m_WorldPos;
        public Optional<GvVec3>        m_ScreenPos;
        public Optional<CursorSkin>    m_Skin;
        public Optional<GvGo>          m_CursorGo;
        public Optional<CursorSet>     m_CursorSet;
        public SoCollection<PointerId> m_States;
        
        private ICursorSkin m_CurrentSkin;
        public ICursorSkin Skin
        {
            set
            {
                if (m_CurrentSkin == value)
                    return;
                
                if (value != null && m_InitStates.Contains(value) == false)
                {
                    m_InitStates.Add(value);
                    value.Init();
                }
                
                m_CurrentSkin = value;
                
                // set default skin if null 
                if (m_CurrentSkin == null && m_CursorSet.Enabled)
                    m_CurrentSkin = m_CursorSet.Value;
            }
        }

        public GameObject Go => m_CursorGo.Value.Value;

        private HashSet<ICursorSkin> m_InitStates = new HashSet<ICursorSkin>();

        // =======================================================================
        [Serializable]
        public class CursorSet : ICursorSkin
        {
            [SoNested]
            public PointerState  m_Drag;
            [SoNested]
            public PointerState  m_Point;
            [SoNested]
            public PointerState  m_Hover;
            [SoNested]
            public PointerState  m_Wait;
            [SoNested]
            public PointerState  m_Idle;
            [SoNested]
            public PointerState  m_View;
            [SoNested]
            public PointerState  m_None;

            private PointerState        m_Current;
            private  List<PointerState> m_States;
        
            // =======================================================================
            public void Init()
            {
                m_States = new List<PointerState>(7);
                m_States.Add(m_Drag);
                m_States.Add(m_Point);
                m_States.Add(m_Wait);
                m_States.Add(m_Hover);
                m_States.Add(m_Idle);
                m_States.Add(m_View);
                m_States.Add(m_None);
                
                m_States.RemoveAll(n=> n == null);
                
                foreach (var cd in m_States)
                    cd.Init();
            
                m_Current = null;
            }

            public void Update()
            {
                var cur = m_States.OrderByDescending(n => n._id._priority).FirstOrDefault(n => n._id._state > 0);
                if (cur == null)
                    cur = m_Idle;
                
                if (m_Current != cur)
                {
                    if (m_Current != null)
                        m_Current.Release();
                    
                    m_Current = cur;
                    m_Current.Assign();
                }
                
                m_Current.Update();
            }
        }

        public interface ICursorSkin
        {
            void Init();
            void Update();
        }
        
        // =======================================================================
        public override void Init()
        {
            m_Clean = new Texture2D(8, 8, TextureFormat.RGBA32, false);
            m_Clean.SetPixels(Enumerable.Repeat(Color.clear, m_Clean.width * m_Clean.height).ToArray());
            m_Clean.Apply();

            // create input for pointer position
            m_PointerPosition = new InputAction(nameof(Pointer), type: InputActionType.PassThrough, binding: "<Pointer>/position", expectedControlType: "Vector2");
            m_PointerPosition.Enable();

            // init ground plane
            switch (m_ProjectSpace)
            {
                case Core.ProjectSpace.XY:
                    GroundPlane = k_GroundPlaneXY;
                    break;
                case Core.ProjectSpace.XZ:
                    GroundPlane = k_GroundPlaneXZ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            m_InitStates.Clear();
            foreach (var cursorId in m_States)
            {
                cursorId._state = 0;
            }
            
            m_CurrentSkin = null;
            if (m_CursorSet.Enabled)
                Skin = m_CursorSet.Value;
            
            if (m_Skin.Enabled)
                Skin = m_Skin.Value;
            
            if (m_CursorGo.Enabled)
            {
                m_CursorGo.Value.Value = Core.Instance._pointer.GetValueOrDefault();
                
                if (m_CursorGo.Value.Value == null)
                {
                    var go = new GameObject("Pointer");
                    go.AddComponent<PointerWorld>();
                    go.transform.SetParent(Core.Instance.transform);
                    m_CursorGo.Value.Value = go;
                    
                    if (Core.Instance._pointer.Enabled)
                        Core.Instance._pointer.Value = go;
                }
            }

            // create updater
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = Update;
        }

        public void Update()
        {                                                            
            // update screen position
            Screen         = m_PointerPosition.ReadValue<Vector2>();
            ScreenUV       = new Vector2(Screen.x / UnityEngine.Screen.width, Screen.y / UnityEngine.Screen.height);
            ScreenUVAspect = new Vector2(ScreenUV.x * (UnityEngine.Screen.width / (float)UnityEngine.Screen.height), ScreenUV.y);

            // update word position
            World = GetWordPosition(GroundPlane);
            Cell  = World.Cell().To2DXY();
            
            m_WorldPositionRef.Value = World;
            
            if (m_WorldPos.Enabled)
                m_WorldPos.Value.Value = World;
            if (m_ScreenPos.Enabled)
                m_ScreenPos.Value.Value = Screen;
            
            if (m_CurrentSkin != null)
                m_CurrentSkin.Update();
        }

        public Vector3 GetWordPosition(Plane plane)
        {
            CameraRay = Core.Camera.ScreenPointToRay(new Vector3(Screen.x, Screen.y, Core.Camera.farClipPlane));

            plane.Raycast(CameraRay, out var d);

            return CameraRay.GetPoint(d);
        }

        public Vector3 GetWordPosition(float distance)
        {
            var ray = Core.Camera.ScreenPointToRay(new Vector3(Screen.x, Screen.y, Core.Camera.farClipPlane));

            return ray.GetPoint(distance);
        }
    }
}