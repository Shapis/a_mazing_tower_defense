using Godot;
using System;
using System.Collections.Generic;

public partial class AStarPathFinder : Node
{
    public AStarPathFinder(TileMap tileMap)
    {
        _tileMap = tileMap;
    }

    private TileMap? _tileMap;

    #region PathNode class
    public class PathNode
    {
        public PathNode(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position;
        public float f;
        public float g;
        public float h;
        public PathNode? previousPathNode;
        public PathNode? UpDestination;
        public PathNode? DownDestination;
        public PathNode? LeftDestination;
        public PathNode? RightDestination;
        public bool IsTravelNode;
        public bool IsAccessible = true;
    }
    #endregion

    public const int _width = 9;

    private const int _height = 14;
    private const int _tileSize = 64;

    private PathNode[,] m_Grid = new PathNode[_width, _height];
    public PathNode[,] Grid
    {
        get => m_Grid;
        private set => m_Grid = value;
    }

    public List<PathNode> GetNodesList()
    {
        List<PathNode> nodes = new List<PathNode>();
        foreach (var item in Grid)
        {
            nodes.Add(item);
        }
        return nodes;
    }

    public void GenerateNavMesh()
    {
        // Generate main grid
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Grid[i, j] = new PathNode(new Vector2(32 + (i * _tileSize), 32 + (j * _tileSize)));
            }
        }

        // Connect main grid
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if (j > 0)
                {
                    Grid[i, j].UpDestination = Grid[i, j - 1];
                }
                if (j < _height - 1)
                {
                    Grid[i, j].DownDestination = Grid[i, j + 1];
                }
                if (i > 0)
                {
                    Grid[i, j].LeftDestination = Grid[i - 1, j];
                }
                if (i < _width - 1)
                {
                    Grid[i, j].RightDestination = Grid[i + 1, j];
                }
            }
        }
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                for (int k = 1; k < 5; k++)
                {
                    if (_tileMap!.GetCellSourceId(k, new Vector2i(i, j), true) != -1)
                    {
                        Grid[i, j].IsAccessible = false;
                    }
                }
            }
        }
    }

    private List<PathNode>? _openList;
    private List<PathNode>? _closedList;

    public List<PathNode>? FindPath(PathNode originNode, PathNode destinationNode)
    {
        // Add origin to the openList
        _openList = new List<PathNode> { originNode };
        _closedList = new List<PathNode>();

        foreach (var item in GetNodesList())
        {
            item.g = int.MaxValue;
            item.h = int.MaxValue;
            item.f = int.MaxValue;
            item.previousPathNode = null;
        }

        originNode.g = 0;
        originNode.h = CalculateDistanceCost(originNode, destinationNode);
        originNode.f = CalculateF(originNode);

        while (_openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(_openList);
            if (currentNode == destinationNode)
            {
                return GetPath(originNode, currentNode);
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);
            foreach (PathNode neighbourNode in GetNeighboursList(currentNode, GetNodesList()))
            {
                if (_closedList.Contains(neighbourNode))
                    continue;

                if (!neighbourNode.IsAccessible)
                {
                    _closedList.Add(neighbourNode);
                    continue;
                }

                float tentativeGCost =
                    currentNode.g + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.g)
                {
                    neighbourNode.previousPathNode = currentNode;
                    neighbourNode.g = tentativeGCost;
                    neighbourNode.h = CalculateDistanceCost(neighbourNode, destinationNode);
                    neighbourNode.f = CalculateF(neighbourNode);

                    if (!_openList.Contains(neighbourNode))
                    {
                        _openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private List<PathNode> GetNeighboursList(PathNode currentNode, List<PathNode> AllNodes)
    {
        List<PathNode> neighbours = new List<PathNode>();

        if (currentNode.UpDestination != null)
        {
            neighbours.Add(currentNode.UpDestination);
        }
        if (currentNode.DownDestination != null)
        {
            neighbours.Add(currentNode.DownDestination);
        }
        if (currentNode.LeftDestination != null)
        {
            neighbours.Add(currentNode.LeftDestination);
        }
        if (currentNode.RightDestination != null)
        {
            neighbours.Add(currentNode.RightDestination);
        }
        return neighbours;
    }

    private List<PathNode>? GetPath(PathNode originNode, PathNode endNode)
    {
        if (originNode.IsAccessible == false)
        {
            return null;
        }

        List<PathNode> path = new List<PathNode> { endNode };

        PathNode currentNode = endNode;

        while (currentNode.previousPathNode != null)
        {
            path.Add(currentNode.previousPathNode);
            currentNode = currentNode.previousPathNode;
        }
        path.Reverse();
        return path;
    }

    private float CalculateF(PathNode node)
    {
        return node.g + node.h;
    }

    private float CalculateDistanceCost(PathNode nodeA, PathNode nodeB)
    {
        return nodeA.Position.DistanceTo(nodeB.Position);
    }

    private PathNode GetLowestFCostNode(List<PathNode> nodes)
    {
        PathNode lowestFCostNode = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].f < lowestFCostNode.f)
            {
                lowestFCostNode = nodes[i];
            }
        }
        return lowestFCostNode;
    }
}
