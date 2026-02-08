using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GodBox.Pathfinding
{
    public static class Pathfinding
    {
        public static List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            PathfindingGrid grid = PathfindingGrid.Instance;
            if (grid == null)
            {
                Debug.LogError("No PathfindingGrid found in scene!");
                return null;
            }

            PathNode startNode = grid.NodeFromWorldPoint(startPos);
            PathNode targetNode = grid.NodeFromWorldPoint(targetPos);

            // If target is unwalkable, try to find a walkable neighbor
            if (!targetNode.Walkable)
            {
                // Simple search for nearest walkable neighbor
                PathNode bestNeighbor = null;
                float minDst = float.MaxValue;
                
                foreach (var n in grid.GetNeighbours(targetNode))
                {
                    if (n.Walkable)
                    {
                        float d = Vector3.Distance(n.WorldPosition, targetNode.WorldPosition);
                        if (d < minDst)
                        {
                            minDst = d;
                            bestNeighbor = n;
                        }
                    }
                }
                
                if (bestNeighbor != null)
                {
                    targetNode = bestNeighbor;
                }
            }

            List<PathNode> openSet = new List<PathNode>();
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // Unoptimized: Find node with lowest FCost
                PathNode currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (PathNode neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
            
            return null;
        }

        static List<Vector3> RetracePath(PathNode startNode, PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            path.Reverse();

            List<Vector3> waypoints = new List<Vector3>();
            foreach (var node in path)
            {
                waypoints.Add(node.WorldPosition);
            }
            return waypoints;
        }

        static int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
