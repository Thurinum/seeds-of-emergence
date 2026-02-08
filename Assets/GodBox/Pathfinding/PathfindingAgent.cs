using System.Collections.Generic;
using UnityEngine;

namespace GodBox.Pathfinding
{
    public class PathfindingAgent : MonoBehaviour
    {
        public float Speed = 5f;
        public float ReachThreshold = 0.1f;
        
        private List<Vector3> _currentPath;
        private int _targetIndex;
        private bool _isMoving;

        [Header("Debug")]
        public string Status;
        public Vector3 TargetPosition;

        public void SetDestination(Vector3 targetPos)
        {
            TargetPosition = targetPos;
            _currentPath = Pathfinding.FindPath(transform.position, targetPos);
            
            if (_currentPath != null && _currentPath.Count > 0)
            {
                _targetIndex = 0;
                _isMoving = true;
                Status = "Moving";
            }
            else
            {
                _isMoving = false;
                Status = _currentPath == null ? "Path Not Found" : "Destination Reached";
                
                // Debugging why path failed
                if (_currentPath == null)
                {
                    PathfindingDebugger.DebugPathRequest(transform.position, targetPos);
                }
            }
        }

        public void Stop()
        {
            _isMoving = false;
            _currentPath = null;
        }

        private void Update()
        {
            if (!_isMoving || _currentPath == null) return;

            Vector3 targetWaypoint = _currentPath[_targetIndex];
            // Flatten Z if 2D
            targetWaypoint.z = transform.position.z; 

            float step = Speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, step);

            if (Vector3.Distance(transform.position, targetWaypoint) < ReachThreshold)
            {
                _targetIndex++;
                if (_targetIndex >= _currentPath.Count)
                {
                    _isMoving = false;
                    _currentPath = null;
                }
            }
        }

        public bool IsMoving => _isMoving;
        
        void OnDrawGizmos()
        {
            if (_currentPath != null)
            {
                for (int i = _targetIndex; i < _currentPath.Count; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(_currentPath[i], Vector3.one * 0.2f);
                    if (i == _targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, _currentPath[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(_currentPath[i - 1], _currentPath[i]);
                    }
                }
            }
        }
    }
}
