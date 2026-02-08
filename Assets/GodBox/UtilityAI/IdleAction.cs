using UnityEngine;
using GodBox.Pathfinding;

namespace GodBox.UtilityAI
{
    [CreateAssetMenu(fileName = "IdleAction", menuName = "GodBox/UtilityAI/Actions/Idle")]
    public class IdleAction : UtilityAction
    {
        public float WanderRadius = 5f;
        public float MinPauseTime = 2f;
        public float MaxPauseTime = 5f;

        public override void Execute(UtilityAIComponent context)
        {
             var mover = context.GetAgentComponent<PathfindingAgent>();
             if (mover == null) return;

             // Check if we are currently waiting
             float nextMoveTime = context.GetData<float>("IdleData_NextMoveTime");
             if (Time.time < nextMoveTime)
             {
                 return; // Just wait
             }

             // If we are moving, let the agent move
             if (mover.IsMoving)
             {
                 return;
             }

             // If we are NOT moving and wait time is passed, it means we just arrived (or first run).
             // Set a new wait time?
             // To avoid "Wait before move" logic blocking us, let's distinguishing "Waiting" from "Ready".
             // Actually, if we just arrived, we want to wait NOW.
             
             // We need to know if we arrived *this frame* or if we have been waiting.
             // Let's use a "State" flag. 0 = Ready, 1 = Moving, 2 = Waiting
             int state = context.GetData<int>("IdleData_State");

             if (state == 1) // Was Moving
             {
                 // We stopped moving, so we arrived.
                 // Start waiting.
                 float pause = Random.Range(MinPauseTime, MaxPauseTime);
                 context.SetData("IdleData_NextMoveTime", Time.time + pause);
                 context.SetData("IdleData_State", 2); // Waiting
                 return;
             }
             else if (state == 2) // Was Waiting
             {
                 // Wait time passed (checked above), so we are Ready.
                 context.SetData("IdleData_State", 0);
             }

             // State 0 (Ready): Pick a new point
             PathNode randomNode = PathfindingGrid.Instance.GetRandomWalkableNode(context.transform.position, WanderRadius);
             if (randomNode != null)
             {
                 mover.SetDestination(randomNode.WorldPosition);
                 context.SetData("IdleData_State", 1); // Moving
             }
             else
             {
                 // Failed, wait briefly
                 context.SetData("IdleData_NextMoveTime", Time.time + 1.0f);
                 context.SetData("IdleData_State", 2);
             }
        }
    }
}
