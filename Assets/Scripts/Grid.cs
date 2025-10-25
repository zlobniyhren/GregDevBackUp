using UnityEngine;

public class Grid : MonoBehaviour
{
    public int height;
    public int lenght;

    int [,] grid;

    public void Init(int height, int lenght)
    {
        grid = new int [height, lenght];
        this.lenght = lenght;
        this.height = height;
    }
    public void Set(int x, int y, int to)
    {
        if (CheckPosition(x, y) == false)
        {
            Debug.LogWarning("Trying to get a cell outside the Grid" + x.ToString() + ":" + y.ToString());
            return;
        }
        grid[x,y] = to;
    }
    public int Get(int x, int y)
    {
         if (CheckPosition(x,y)==false)
        {
            Debug.LogWarning("Trying to get a cell outside the Grid" + x.ToString() + ":" + y.ToString());
            return -1;
        }
        return grid[x,y];
    }
    
    public bool CheckPosition(int x, int y)
    {
        if (x < 0 ||  x >= lenght)
        {
            return false;
        }
        if (y < 0 ||  y >= height)
        {
            return false;
        }

        return true;
    }
}
 