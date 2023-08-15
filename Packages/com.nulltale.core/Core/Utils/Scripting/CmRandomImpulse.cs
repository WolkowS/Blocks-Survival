using Cinemachine;
using UnityEngine;

namespace CoreLib.Scripting
{
    public class CmRandomImpulse : MonoBehaviour
    {
        public void Invoke()
        {
            var source = GetComponent<CinemachineImpulseSource>();
            source.GenerateImpulse((UnityRandom.Normal2D() * source.m_DefaultVelocity.magnitude).To3DXY());
        }
    }
}