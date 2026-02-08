using System.Collections.Generic;
using UnityEngine;

namespace GodBox.Pathfinding
{
    public class PathfindingGrid : MonoBehaviour
    {
        public static PathfindingGrid Instance;

        [Tooltip("Layer mask for objects that should block the path.")]
        public LayerMask ObstacleLayerMask;
        public Vector2 GridWorldSize;
        public float NodeRadius;
        
        PathNode[,] _grid;
        float _nodeDiameter;
        int _gridSizeX, _gridSizeY;

        private void Awake()
        {
            Instance = this;
            _nodeDiameter = NodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt(GridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(GridWorldSize.y / _nodeDiameter);
            CreateGrid();
        }

        public int MaxSize => _gridSizeX * _gridSizeY;

        private void CreateGrid()
        {
            _grid = new PathNode[_gridSizeX, _gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.up * GridWorldSize.y / 2;

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + NodeRadius) + Vector3.up * (y * _nodeDiameter + NodeRadius);
                    // use slightly smaller radius to avoid checking neighbor cells
                    bool walkable = !Physics2D.OverlapCircle(worldPoint, NodeRadius * 0.9f, ObstacleLayerMask);
                    _grid[x, y] = new PathNode(walkable, worldPoint, x, y);
                }
            }
        }

        public PathNode NodeFromWorldPoint(Vector3 worldPosition)
        {
            float percentX = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
            float percentY = (worldPosition.y + GridWorldSize.y / 2) / GridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
            return _grid[x, y];
        }

        public List<PathNode> GetNeighbours(PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;

                    if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
                    {
                        neighbours.Add(_grid[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, GridWorldSize.y, 1));

            if (_grid != null)
            {
                foreach (PathNode n in _grid)
                {
                    Gizmos.color = (n.Walkable) ? Color.white : Color.red;
                    Gizmos.DrawCube(n.WorldPosition, Vector3.one * (_nodeDiameter - .1f));
                }
            }
        }
    }
}
