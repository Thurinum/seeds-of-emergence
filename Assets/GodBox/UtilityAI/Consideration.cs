using System;
using UnityEngine;

namespace GodBox.UtilityAI
{
    [Serializable]
    public abstract class Consideration
    {
        public string Name = "Consideration";
        public AnimationCurve ResponseCurve = AnimationCurve.Linear(0, 0, 1, 1);

        public float Evaluate(UtilityAIComponent context)
        {
            float input = GetInput(context);
            return ResponseCurve.Evaluate(Mathf.Clamp01(input));
        }

        public abstract float GetInput(UtilityAIComponent context);
    }
}
