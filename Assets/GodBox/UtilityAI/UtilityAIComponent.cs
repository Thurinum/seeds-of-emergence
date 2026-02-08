using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodBox.UtilityAI
{
    public class UtilityAIComponent : MonoBehaviour
    {
        public float TickInterval = 0.5f;
        public List<UtilityAction> AvailableActions;

        private float _timeSinceLastTick;
        // Expose current action for debug/inspection if needed
        [SerializeField] private UtilityAction _currentAction; 

        // Simple Blackboard
        private Dictionary<string, object> _blackboard = new Dictionary<string, object>();
        public void SetData(string key, object value) => _blackboard[key] = value;
        public object GetData(string key) => _blackboard.ContainsKey(key) ? _blackboard[key] : null;
        public T GetData<T>(string key) => _blackboard.ContainsKey(key) ? (T)_blackboard[key] : default(T);

        public UtilityAction CurrentAction => _currentAction;

        private void Update()
        {
            _currentAction?.Execute(this);

            _timeSinceLastTick += Time.deltaTime;
            if (_timeSinceLastTick >= TickInterval)
            {
                _timeSinceLastTick = 0;
                Tick();
            }
        }

        private void Tick()
        {
            UtilityAction bestAction = null;
            float bestScore = -1f;

            if (AvailableActions != null)
            {
                foreach (var action in AvailableActions)
                {
                    if (action == null) continue;
                    
                    float score = action.Evaluate(this);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestAction = action;
                    }
                }
            }

            if (bestAction != _currentAction)
            {
                _currentAction = bestAction;
            }
        }
        
        public T GetAgentComponent<T>() where T : Component
        {
            return GetComponent<T>();
        }
    }
}
