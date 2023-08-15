using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class AddLerp
    {
        public float _move;
        public float _lerp;
        
        // =======================================================================
        public Vector2 Evaluate(Vector2 pos, Vector2 goal, float deltaTime)
        {
            var result = Vector2.Lerp(pos, goal, _lerp * deltaTime);
            result = Vector2.MoveTowards(result, goal, _move * deltaTime);
            
            return result;
        }
        public Vector3 Evaluate(Vector3 pos, Vector3 goal, float deltaTime)
        {
            var result = Vector3.Lerp(pos, goal, _lerp * deltaTime);
            result = Vector3.MoveTowards(result, goal, _move * deltaTime);
            
            return result;
        }
    }
}