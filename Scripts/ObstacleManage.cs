using UnityEngine;

public class ObstacleManage : MonoBehaviour
{
    public GridObstacleData obstacleData;   // Reference to the grid scriptableobject
    public GameObject obstaclePrefab;       // Assign the sphere (obstacle) prefab
    public float step = 1.0f;

    void Start()
    {
        GenerateObstacles();
    }

    void GenerateObstacles()
    {
        int gridSize = obstacleData.gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (obstacleData.IsBlocked(x, z))
                {
                    Vector3 position = new Vector3(x * step, 0.2f, z * step);
                    Instantiate(obstaclePrefab, position, Quaternion.identity); // spawning sphere obstacles on the saved tiles
                }
            }
        }
    }
}