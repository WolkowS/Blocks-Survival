using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;
using Debug = UnityEngine.Debug;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(DebugTools), menuName = Core.k_CoreModuleMenu + nameof(DebugTools))]
    public class DebugTools : Core.Module<DebugTools>
    {
        public Vector2     m_TextOffset = new Vector2(16, 8);
        public float m_LineHeight = 18;
        [SerializeField]
        private bool m_Show = true;
        public bool Show
        {
            get => m_Show;
            set => m_Show = value;
        }

        [SerializeField]
        private Color m_TextColor = Color.white;
        [SerializeField]
        private Optional<Color> m_TextOutileColor = new Optional<Color>(Color.black, false);
        [SerializeField]
        private Color m_GizmoColor = Color.yellow;

        private List<Drawer> m_TextDrawers  = new List<Drawer>();
        private List<Drawer> m_GizmoDrawers = new List<Drawer>();

        private static Rect   s_TextRect;
        private static Drawer s_NullDrawer = new Drawer();

        // =======================================================================
        public class Drawer
        {
            internal Action<Drawer> m_Action;
            internal string         m_Text;
            internal float          m_Duration;
            internal Color          m_Color;
            internal Matrix4x4      m_Matrix = Matrix4x4.identity;

            // =======================================================================
            internal void Draw()
            {
                m_Action.Invoke(this);
            }

            public Drawer Duration(float duration)
            {
                m_Duration = duration;
                return this;
            }

            public Drawer Text(string text)
            {
                m_Text = text;
                return this;
            }

            public Drawer Color(Color color)
            {
                m_Color = color;
                return this;
            }
            
            public Drawer Matrix(Matrix4x4 matrix)
            {
                m_Matrix = matrix;
                return this;
            }
        }

        internal class DrawerUpdate : MonoBehaviour
        {
            internal Action m_OnGUI;
            internal Action m_OnGizmos;
	    
            // =======================================================================
            private void Awake()
            {
                hideFlags = HideFlags.DontSave;
            }

            private void OnGUI()
            {
                m_OnGUI.Invoke();
            }

            private void OnDrawGizmos()
            {
                m_OnGizmos.Invoke();
            }
        }
        
        // =======================================================================
        public override void Init()
        {
            m_TextDrawers  = new List<Drawer>();
            
            m_GizmoDrawers = new List<Drawer>();

            var drawerUpdate = Core.Instance.gameObject.AddComponent<DrawerUpdate>();
            drawerUpdate.m_OnGUI    = _drawGUI;
            drawerUpdate.m_OnGizmos = _drawGizmos;
        }

        private void _drawGizmos()
        {
            // draw
            if (Show)
            {
                foreach (var drawer in m_GizmoDrawers)
                {
                    drawer.Draw();
                    drawer.m_Duration -= Time.unscaledTime;

                    if (drawer.m_Duration < 0f)
                        GenericPool<Drawer>.Release(drawer);
                }
            }

            // clear expired
            m_GizmoDrawers.RemoveAll(n => n.m_Duration < 0f);
        }

        private void _drawGUI()
        {
            // draw
            if (Show && Event.current.type == EventType.Repaint)
            {
                s_TextRect = new Rect(m_TextOffset.x, m_TextOffset.y, Screen.width, Screen.height);

                foreach (var drawer in m_TextDrawers)
                {
                    drawer.Draw();
                    drawer.m_Duration -= Time.unscaledTime;

                    s_TextRect.y += m_LineHeight;

                    if (drawer.m_Duration < 0f)
                        GenericPool<Drawer>.Release(drawer);
                }
            }

            // clear expired
            m_TextDrawers.RemoveAll(n => n.m_Duration < 0f);
        }
        
        // =======================================================================
        public static Drawer DrawLine(Vector3 from, Vector3 to)
        {
            return _drawGizmo(drawer =>
            {
                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Gizmos.matrix;
                Gizmos.DrawLine(from, to);
            });
        }

        public static Drawer DrawCube(Vector3 pos, Vector3 size)
        {
            return _drawGizmo(drawer =>
            {
                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Gizmos.matrix;
                Gizmos.DrawCube(pos, size);
            });
        }

        public static Drawer DrawWireCube(Vector3 pos, Vector3 size)
        {
            return _drawGizmo(drawer =>
            {
                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Gizmos.matrix;
                Gizmos.DrawWireCube(pos, size);
            });
        }

        public static Drawer DrawSphere(Vector3 pos, float radius)
        {
            return _drawGizmo(drawer =>
            {
                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Gizmos.matrix;
                Gizmos.DrawSphere(pos, radius);
            });
        }

        public static Drawer DrawWireSphere(Vector3 pos, float radius)
        {
            return _drawGizmo(drawer =>
            {
                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Gizmos.matrix;
                Gizmos.DrawWireSphere(pos, radius);
            });
        }

        public static Drawer DrawDisk(Vector3 pos, Quaternion rot, float radius)
         {
            return _drawGizmo(drawer =>
            {
                const float DISK_THICKNESS = 0.01f;

                Gizmos.color  = drawer.m_Color;
                Gizmos.matrix = Matrix4x4.TRS(pos, rot, new Vector3(1, DISK_THICKNESS, 1)) * drawer.m_Matrix;
                Gizmos.DrawSphere(Vector3.zero, radius);
            });
         }

        public static void Log(string text)
        {
            Debug.Log(text);
        }
        
        public static void ScreenLog(string text)
        {
            if (Instance.m_Show == false)
                return;
            
            var textDrawer = GenericPool<Drawer>.Get();
            textDrawer.m_Color    = Instance.m_TextColor;
            textDrawer.m_Duration = 0f;
            textDrawer.m_Text     = text;

            textDrawer.m_Action = drawer =>
            {
                if (Instance.m_TextOutileColor.Enabled)
                {
                    GUI.color = Instance.m_TextOutileColor.Value;
                    GUI.Label(s_TextRect.IncX(2), drawer.m_Text);
                    GUI.Label(s_TextRect.IncX(-2), drawer.m_Text);
                    GUI.Label(s_TextRect.IncY(2), drawer.m_Text);
                    GUI.Label(s_TextRect.IncY(-2), drawer.m_Text);
                }
                
                GUI.color = drawer.m_Color;
                GUI.Label(s_TextRect, drawer.m_Text);
            };

            Instance.m_TextDrawers.Add(textDrawer);
        }

        public static void ScreenLog(string name, object value)
        {
            ScreenLog($"{name} :{value}");
        }

        public static void ScreenLog(object value)
        {
            ScreenLog(value.ToString());
        }

        // -----------------------------------------------------------------------
        private static Drawer _createGizmoDrawer()
        {
            var gizmoDrawer = GenericPool<Drawer>.Get();
            gizmoDrawer.m_Color    = Instance.m_GizmoColor;
            gizmoDrawer.m_Duration = 0f;
            gizmoDrawer.m_Matrix   = Matrix4x4.identity;
            Instance.m_GizmoDrawers.Add(gizmoDrawer);
            return gizmoDrawer;
        }

        private static Drawer _drawGizmo(Action<Drawer> action)
        {
#if DEBUG
            var gizmoDrawer = _createGizmoDrawer();

            gizmoDrawer.m_Action = action;

            return gizmoDrawer;
#else
            return s_NullDrawer;
#endif
        }
    }

    public static class DebugToolsExtensions
    {
        public static DebugTools.Drawer DrawCube(this Transform t, Vector3 size, Vector3 offset = default)
        {
            return DebugTools.DrawCube(t.position + offset, size);
        }
        
        public static DebugTools.Drawer DrawWireCube(this Transform t, Vector3 size, Vector3 offset = default)
        {
            return DebugTools.DrawWireCube(t.position + offset, size);
        }

        public static DebugTools.Drawer DrawSphere(this Transform t, float size, Vector3 offset = default)
        {
            return DebugTools.DrawSphere(t.position + offset, size);
        }

        public static DebugTools.Drawer DrawWireSphere(this Transform t, float size, Vector3 offset = default)
        {
            return DebugTools.DrawWireSphere(t.position + offset, size);
        }

        public static DebugTools.Drawer DrawDisc(this Transform t, float size, Vector3 offset = default)
        {
            return DebugTools.DrawDisk(t.position + offset, t.rotation, size);
        }

        // -----------------------------------------------------------------------
        public static DebugTools.Drawer DrawCube(this Component c, Vector3 size, Vector3 offset = default)
        {
            return c.transform.DrawCube(size, offset);
        }
        
        public static DebugTools.Drawer DrawWireCube(this Component c, Vector3 size, Vector3 offset = default)
        {
            return c.transform.DrawWireCube(size, offset);
        }

        public static DebugTools.Drawer DrawSphere(this Component c, float size, Vector3 offset = default)
        {
            return c.transform.DrawSphere(size, offset);
        }

        public static DebugTools.Drawer DrawWireSphere(this Component c, float size, Vector3 offset = default)
        {
            return c.transform.DrawWireSphere(size, offset);
        }

        public static DebugTools.Drawer DrawDisc(this Component c, float size, Vector3 offset = default)
        {
            return c.transform.DrawDisc(size, offset);
        }

        public static void DrawText(this Component c, string text)
        {
            DebugTools.ScreenLog(text, c);
        }

        public static void DrawText(this object c)
        {
            DebugTools.ScreenLog(c);
        }
    }
}