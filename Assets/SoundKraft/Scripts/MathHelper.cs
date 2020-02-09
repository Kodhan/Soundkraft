using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;
namespace SoundKraft
{
    [ExecuteInEditMode]
    public class MathHelper
    {
        [Serializable]
        public struct FloatMinMax
        {

            public Vector2 MinMax;
            public FloatMinMax(float min, float max, float currentMin = 1, float currentMax = 1)
            {
                MinMax = new Vector2(min, max);
            }
            private float GetFloat()
            {
                return (Random.Range(MinMax.x, MinMax.y));
            }
            public static implicit operator float(FloatMinMax minMax)
            {
                return minMax.GetFloat();
            }

            [ContextMenu("Test")]
            public void Test()
            {
                Debug.Log("Min: " + MinMax.x + ", Max: " + MinMax.y);
            }

            internal float GetRandom()
            {
                return Random.Range(MinMax.x, MinMax.y);
            }

            internal float Evaluate(float factor)
            {
                return MinMax.x + ((MinMax.y - MinMax.x) * Mathf.Clamp01(factor));
            }
        }


    }
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min = 0;
        public float Max = 1;
        [HideInInspector] public bool ShowEditRange;
        [HideInInspector] public bool ShowDebugValues;

        public MinMaxAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}