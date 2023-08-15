using System.Collections.Generic;

namespace CoreLib
{
    public static class StaticPool<T> where T : new()
    {
        public static Stack<T> s_Stack = new Stack<T>();
        
        // =======================================================================
        public static T Get()
        {
            if (s_Stack.Count > 0)
                return s_Stack.Pop();
            
            return new T();
        }

        public static void Release(T item)
        {
            s_Stack.Push(item);
        }
    }
}