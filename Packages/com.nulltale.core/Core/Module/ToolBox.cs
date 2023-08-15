using UnityEngine;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(ToolBox), menuName = Core.k_CoreModuleMenu + nameof(ToolBox))]
    public class ToolBox : Core.Module<ToolBox>
    {
        public override void Init()
        {
        }
        
        public void Log(string text)
        {
            Debug.Log(text);
        }
        
        public void Log(int num)
        {
            Debug.Log(num);
        }

        public void UnParent(Transform trans)
        {
            trans.SetParent(null);
        }
        
        public new void Destroy(Object obj)
        {
            Object.Destroy(obj);
        }
    }
}