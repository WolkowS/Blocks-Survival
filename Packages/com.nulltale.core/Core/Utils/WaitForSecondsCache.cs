using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    public static class WaitForSecondsCache
    {
        private static Dictionary<float, WaitForSeconds>         s_WaitForSecondsCache         = new Dictionary<float, WaitForSeconds>(64);
        private static Dictionary<float, WaitForSecondsRealtime> s_WaitForSecondsRealtimeCache = new Dictionary<float, WaitForSecondsRealtime>(64);

        // =======================================================================
        public static WaitForSeconds Scaled(float sec)
        {
            if (s_WaitForSecondsCache.TryGetValue(sec, out var value) == false)
            {
                value = new WaitForSeconds(sec);
                s_WaitForSecondsCache.Add(sec, value);
            }

            return value;
        }
        
        public static WaitForSecondsRealtime Realtime(float sec)
        {
            if (s_WaitForSecondsRealtimeCache.TryGetValue(sec, out var value) == false)
            {
                value = new WaitForSecondsRealtime(sec);
                s_WaitForSecondsRealtimeCache.Add(sec, value);
            }

            return value;
        }
    }
}