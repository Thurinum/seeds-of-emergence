using System;
using UnityEngine;

namespace GodBox.UtilityAI.Considerations
{
    [Serializable]
    public class FixedConsideration : Consideration
    {
        [Range(0,1)]
        public float Value = 1f;

        public override float GetInput(UtilityAIComponent context)
        {
            return Value;
        }
    }

    [Serializable]
    public class HungerConsideration : Consideration
    {
        public override float GetInput(UtilityAIComponent context)
        {
            var needs = context.GetAgentComponent<BasicNeedsComponent>();
            if (needs == null) return 0f;
            return 1f - needs.FoodPercentage;
        }
    }

    [Serializable]
    public class HealthConsideration : Consideration
    {
        public override float GetInput(UtilityAIComponent context)
        {
            var needs = context.GetAgentComponent<BasicNeedsComponent>();
            if (needs == null) return 0f;
            return needs.HealthPercentage;
        }
    }

    [Serializable]
    public class BlackboardFloatConsideration : Consideration
    {
        public string Key;
        
        public override float GetInput(UtilityAIComponent context)
        {
            if (string.IsNullOrEmpty(Key)) return 0f;
            return context.GetData<float>(Key);
        }
    }

    [Serializable]
    public class NoiseConsideration : Consideration
    {
        public float Frequency = 0.1f;
        public float Offset = 0f;

        public override float GetInput(UtilityAIComponent context)
        {
            // Use instance ID to ensure different agents have different patterns
            float agentOffset = (context.GetInstanceID() % 100) / 100f * 100f; 
            float t = Time.time * Frequency + agentOffset + Offset;
            return Mathf.PerlinNoise(t, 0f);
        }
    }
}
