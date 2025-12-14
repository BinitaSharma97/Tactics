using UnityEngine;

[CreateAssetMenu(fileName = "GridObstacleData", menuName = "Scriptable Objects/GridObstacleData")]
public class GridObstacleData : ScriptableObject
{
    public int gridSize = 10;
    public bool[] blockedTiles = new bool[100]; //assigning the grid

    public bool IsBlocked(int x, int y)
    {
        return blockedTiles[y * gridSize + x];
    }

    public void SetBlocked(int x, int y, bool value)
    {
        blockedTiles[y * gridSize + x] = value;
    }

}
