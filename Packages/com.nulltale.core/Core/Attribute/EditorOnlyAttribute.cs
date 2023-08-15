using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EditorOnlyAttribute : PropertyAttribute
    {
    }
}