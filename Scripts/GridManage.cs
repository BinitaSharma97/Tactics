using System.Collections.Generic;
using UnityEngine;

public class GridManage : MonoBehaviour
{
    public GameObject cubePrefab; // PRefab
    public int gridSize = 10;
    public float step = 1.0f; // Space between tiles

    void Start()
    {
        GridGen();
    }

    void GridGen()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float posX = x * step;
                float posZ = z * step;

                GameObject cube = Instantiate(cubePrefab, new Vector3(posX, 0, posZ), Quaternion.identity); //Generating tiles in the given position
                cube.name = $"Tile_{posX}_{posZ}"; // defining string

                TilesInform info = cube.AddComponent<TilesInform>(); // getting Tiles information from the tilecode

            }
        }
    }

}
