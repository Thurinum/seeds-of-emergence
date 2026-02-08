using System.Collections.Generic;
using UnityEngine;

namespace GodBox.Pathfinding
{
    public class PathfindingDebugger : MonoBehaviour
    {
        public static PathfindingDebugger Instance;

        public Vector3 LastStart;
        public Vector3 LastTarget;
        
        [Header("Status")]
        public bool StartNodeWalkable;
        public bool TargetNodeWalkable;
        public string Result;

        private void Awake()
        {
            Instance = this;
        }

        public static void DebugPathRequest(Vector3 start, Vector3 target)
        {
            if (Instance == null)
            {
                var go = new GameObject("PathfindingDebugger");
                Instance = go.AddComponent<PathfindingDebugger>();
            }

            Instance.LastStart = start;
            Instance.LastTarget = target;
            Instance.CheckRequest();
        }

        private void CheckRequest()
        {
            if (PathfindingGrid.Instance == null)
            {
                Result = "No Grid Found";
                return;
            }

            var startNode = PathfindingGrid.Instance.NodeFromWorldPoint(LastStart);
            var targetNode = PathfindingGrid.Instance.NodeFromWorldPoint(LastTarget);

            StartNodeWalkable = startNode != null && startNode.Walkable;
            TargetNodeWalkable = targetNode != null && targetNode.Walkable;

            if (!StartNodeWalkable && !TargetNodeWalkable)
                Result = "Both Start and Target Unwalkable";
            else if (!StartNodeWalkable)
                Result = "Start Unwalkable";
            else if (!TargetNodeWalkable)
                Result = "Target Unwalkable";
            else
                Result = "Nodes Walkable (Path blocked en route?)";
        }

        private void OnDrawGizmos()
        {
            if (PathfindingGrid.Instance == null) return;

            // Draw Start
            Gizmos.color = StartNodeWalkable ? Color.green : Color.red;
            Gizmos.DrawWireSphere(LastStart, 0.5f);
            
            // Draw Target
            Gizmos.color = TargetNodeWalkable ? Color.green : Color.red;
            Gizmos.DrawWireSphere(LastTarget, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(LastStart, LastTarget);
        }
    }
}
