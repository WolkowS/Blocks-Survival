using System.Linq;
using SoCreator;
using UnityEngine;

namespace CoreLib.Module
{
    [SoCreate(true)]
    public class CursorSkin : ScriptableObject, Pointer.ICursorSkin
    {
        public  SoCollection<PointerState> m_States;
        public  PointerState               m_Idle;
        private PointerState               m_Current;
        
        // =======================================================================
        public void Init()
        {
            foreach (var cd in m_States)
                cd.Init();
            
            m_Current = null;
        }
        
        public void Update()
        {
            var cur = m_States.Values.OrderByDescending(n => n._id._priority).FirstOrDefault(n => n._id._state > 0);
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
}