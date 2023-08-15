using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    public static class MouseButton
	{
		public const int Left = 0;
		public const int Right = 1;
		public const int Middle = 2;
	}

    public abstract class EventWindow : EditorWindow
    {
        protected Dictionary<EventType, Action<Event>> m_EventTable;

        // =======================================================================
        protected EventWindow()
        {
            m_EventTable = new Dictionary<EventType, Action<Event>> {
                { EventType.MouseDown,       OnMouseDown       },
                { EventType.MouseUp,         OnMouseUp         },
                { EventType.MouseDrag,       OnMouseDrag       },
                { EventType.MouseMove,       OnMouseMove       },
                { EventType.ScrollWheel,     OnScrollWheel     },
                { EventType.ContextClick,    OnContextClick    },
                { EventType.KeyDown,         OnKeyDown         },
                { EventType.KeyUp,           OnKeyUp           },
                { EventType.ValidateCommand, OnValidateCommand },
                { EventType.ExecuteCommand,  OnExecuteCommand  },
            };
        }
		
        protected virtual void OnMouseDown(Event e) { }
        protected virtual void OnMouseUp(Event e)   { }
        protected virtual void OnMouseDrag(Event e) { }
        protected virtual void OnMouseMove(Event e) { }
        protected virtual void OnScrollWheel(Event e) { }
        protected virtual void OnContextClick(Event e) { }
        protected virtual void OnKeyDown(Event e) { }
        protected virtual void OnKeyUp(Event e) { }
        protected virtual void OnValidateCommand(Event e) { }
        protected virtual void OnExecuteCommand(Event e) { }
		
        protected virtual void HandleEvents(Event e)
        {
            if (m_EventTable.TryGetValue(e.type, out var handler))
                handler.Invoke(e);
        }
    }
}