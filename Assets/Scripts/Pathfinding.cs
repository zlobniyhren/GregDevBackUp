using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
using Unity.AI.Navigation;
using UnityEditor.Search;
using Unity.VisualScripting;

public class PathNode
{
    public int xPos;
    public int yPos;
    public int gValue;
    public int hValue;
    public PathNode parentNode;
    public int fValue
    {
        get
        {
            return gValue + hValue;
        }
    }
    public PathNode(int xPos, int yPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
    }
 }
 public class GridMap
{
    public int width, height;
    public float cellSize;
    private bool[,] walkable;

    public GridMap(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        walkable = new bool[width, height];

        // по умолчанию все клетки проходимые
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                walkable[x, y] = true;
    }

    public Vector3 GetWorldPosition(int x, int y)
        => new Vector3((x + 0.5f) * cellSize, 0f, (y + 0.5f) * cellSize);

    public bool InBounds(int x, int y)
        => x >= 0 && y >= 0 && x < width && y < height;

    public bool IsWalkable(int x, int y)
        => walkable[x, y];

    public bool CheckWalkable(int xPos, int yPos)
    {
        return walkable[xPos, yPos];
    }

    public void SetWalkable(int x, int y, bool value)
    {
        if (InBounds(x, y)) walkable[x, y] = value;
    }

    /// <summary>
    /// Возвращает true, если клетка (x,y) внутри карты и проходима.
    /// </summary>
    public bool CheckPosition(int x, int y)
        => InBounds(x, y) && IsWalkable(x, y);
}

[RequireComponent(typeof(Grid))]

public class Pathfinding : MonoBehaviour
{
    GridMap gridMap;
    PathNode[,] pathNodes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (gridMap == null)
        {
            gridMap = GetComponent<GridMap>();
        }
        pathNodes = new PathNode[gridMap.width, gridMap.height];
        for (int x = 0; x < gridMap.width; x++)
        {
            for (int y = 0; y < gridMap.height; y++)
            {
                pathNodes[x, y] = new PathNode(x, y);
            }
        }
    }
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = pathNodes[startX, startY];
        PathNode endNode = pathNodes[endX, endY];
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (currentNode.fValue > openList[i].fValue)
                {
                    currentNode = openList[i];
                }
                if (currentNode.fValue == openList[i].fValue && currentNode.hValue > openList[i].hValue)
                {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            List<PathNode> neighbourNodes = new List<PathNode>();
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    if (gridMap.CheckPosition(currentNode.xPos + x, currentNode.yPos + y) == false)
                    {
                        continue;
                    }
                    neighbourNodes.Add(pathNodes[currentNode.xPos + x, currentNode.yPos + y]);

                }
            }
            for (int i = 0; i < neighbourNodes.Count; i++)
            {
                if (closedList.Contains(neighbourNodes[i]))
                {
                    continue;
                }
                if (gridMap.CheckWalkable(neighbourNodes[i].xPos, neighbourNodes[i].yPos) == false)
                {
                    continue;
                }
                int movementCost = currentNode.gValue + CalculateDistance(currentNode, neighbourNodes[i]);
                if (openList.Contains(neighbourNodes[i]) == false || movementCost < neighbourNodes[i].gValue)
                {
                    neighbourNodes[i].gValue = movementCost;
                    neighbourNodes[i].hValue = CalculateDistance(neighbourNodes[i], endNode);
                    neighbourNodes[i].parentNode = currentNode;

                    if (openList.Contains(neighbourNodes[i]) == false)
                    {
                        openList.Add(neighbourNodes[i]);
                    }
                }

            }
            return null; //Костыль
        }
        return null; //Это костыль
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(PathNode current, PathNode target)
    {
        int distX = Math.Abs(current.xPos - target.xPos);
        int distY = Math.Abs(current.yPos - target.yPos);
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX); 
    }
}
