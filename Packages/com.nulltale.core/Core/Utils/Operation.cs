using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public enum Operation
    {
        Add,
        Substract,
        Multiply,
        Divide,
        Power,
        None
    }
    
    public static class OperationExtentions
    {
        public static float Apply(this Operation operation, float a, float b)
        {
            return operation switch
            {
                Operation.Add       => a + b,
                Operation.Substract => a - b,
                Operation.Multiply  => a * b,
                Operation.Divide    => a / b,
                Operation.Power     => Mathf.Pow(a, b),
                Operation.None      => a,
                _                   => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }
    }
}