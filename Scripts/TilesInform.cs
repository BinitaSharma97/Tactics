using UnityEngine;

public class TilesInform : MonoBehaviour
{
    public float x;
    public float z;

    //Tile Values
    void Start()
    {
        Vector3 pos = transform.position;
        x = pos.x;
        z = pos.z;
    }

}
