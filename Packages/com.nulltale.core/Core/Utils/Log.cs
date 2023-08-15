using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class Log : MonoBehaviour
    {
        [SerializeField] [ResizableTextArea]
        private string m_Text;

        [SerializeField]
        private Type m_Type = Type.Message;

        // =======================================================================
        [Serializable]
        public enum Type
        {
            Message,
            Warning,
            Error
        }

        // =======================================================================
        public void Invoke() => Invoke(m_Text);
        public void Invoke(string text)
        {
            switch (m_Type)
            {
                case Type.Message:
                    Debug.Log(text, this);
                    break;
                case Type.Warning:
                    Debug.LogWarning(text, this);
                    break;
                case Type.Error:
                    Debug.LogError(text, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}