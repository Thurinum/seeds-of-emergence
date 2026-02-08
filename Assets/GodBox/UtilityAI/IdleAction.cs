using UnityEngine;

namespace GodBox.UtilityAI
{
    [CreateAssetMenu(fileName = "IdleAction", menuName = "GodBox/UtilityAI/Actions/Idle")]
    public class IdleAction : UtilityAction
    {
        public override void Execute(UtilityAIComponent context)
        {
             // Do nothing
        }
    }
}
