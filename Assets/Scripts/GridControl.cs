using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridControl : MonoBehaviour
{
  [SerializeField] Tilemap targetTileMap;
  [SerializeField] GridManager gridManager;
  Pathfinding pathfinding;
  int currentX = 0;
  int currentY = 0;
  int targetPosX = 0;
  int targetPosY = 0;
  [SerializeField] TileBase highlightTile;

  private void Update()
  {
    MouseInput();
    pathfinding = gridManager.GetComponent<Pathfinding>();
  }
  private void MouseInput()
  {
    if (Input.GetMouseButtonDown(0))
    {
      Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Vector3Int clickPosition = targetTileMap.WorldToCell(worldPoint);
      //gridManager.Set(clickPosition.x, clickPosition.y,1);
      Debug.Log(clickPosition);
      targetPosX = clickPosition.x;
      targetPosY = clickPosition.y;
      List<PathNode> path = pathfinding.FindPath(currentX, currentY, targetPosX, targetPosY);
      if (path != null)
      {
        for (int i = 0; i < path.Count; i++)
        {
          targetTileMap.SetTile(new Vector3Int(path[i].xPos, path[i].yPos, 0), highlightTile);
        }
        currentX = targetPosX;
        currentY = targetPosY;
      }
    }
  }
}
