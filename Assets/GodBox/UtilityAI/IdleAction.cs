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

             // Use blackboard to store state: "IdleData_NextMoveTime", "IdleData_IsWaiting"
             float nextMoveTime = context.GetData<float>("IdleData_NextMoveTime");
             
             if (Time.time < nextMoveTime) {
                 mover.Stop();
                 return;
             }

             if (!mover.IsMoving)
             {
                // We reached destination or haven't started. Pick a new one?
                // Or maybe we just arrived. If just arrived, set a pause time.
                // Since this Execute runs every frame, "Not Moving" means we are standing still.
                
                // Let's assume if we are not moving, we need to pick a spot OR wait.
                // Simple state machine via blackboard: 
                // 1. If Status == Moving -> Do nothing (PathfindingAgent handles it)
                // 2. If Status == Stopped -> Set Wait Timer.
                
                // However, PathfindingAgent.IsMoving is true while traversing. 
                // So if !IsMoving, we are done.

                // Pick random point
                PathNode randomNode = PathfindingGrid.Instance.GetRandomWalkableNode(context.transform.position, WanderRadius);
                if (randomNode != null)
                {
                    mover.SetDestination(randomNode.WorldPosition);
                    // Set next pause to happen AFTER we arrive? 
                    // No, set a timeout so we don't spam pathfinding if we get stuck, 
                    // but also we need logic to pause AFTER arrival.
                    
                    // Actually, simpler:
                    // If not moving, calculate random delay, THEN move.
                    float pause = Random.Range(MinPauseTime, MaxPauseTime);
                    context.SetData("IdleData_NextMoveTime", Time.time + pause);
                }
             }
        }
    }
}
