using CoreLib;
using UnityEngine;
using UnityEngine.Events;

public class SecToString : MonoBehaviour
{
    public UnityEvent<string> m_OnCast;

    // =======================================================================
    public void Invoke(float sec)
    {
        sec = sec.ClampDown(0);
        
        var output = $"{(int)(sec % 60)}s";
        if (sec >= 60f)
            output = $"{(sec / 60).FloorToInt()}m {output}";
        
        m_OnCast.Invoke(output);
    }
}