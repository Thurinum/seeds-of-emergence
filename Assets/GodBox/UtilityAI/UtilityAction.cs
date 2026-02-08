using System.Collections.Generic;
using UnityEngine;

namespace GodBox.UtilityAI
{
    public abstract class UtilityAction : ScriptableObject
    {
        public string ActionName;
        
        [SerializeReference]
        public List<Consideration> Considerations = new List<Consideration>();

        public float Evaluate(UtilityAIComponent context)
        {
            if (Considerations == null || Considerations.Count == 0)
                return 0f;

            float score = 1f;
            foreach (var consideration in Considerations)
            {
                if (consideration == null) continue;
                score *= consideration.Evaluate(context);
                if (score <= 0.001f) return 0; // Optimization
            }
            
            return score;
        }

        public abstract void Execute(UtilityAIComponent context);
    }
}
