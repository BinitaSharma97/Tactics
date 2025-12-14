using UnityEngine;
using UnityEngine.UI;

public class TileSelect : MonoBehaviour
{
    public Camera cam;
    public Text tileinfotext;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); // Raycast mouse
        RaycastHit hit; 

        if (Physics.Raycast(ray, out hit))
        {
            TilesInform tile = hit.collider.GetComponent<TilesInform>();
            if (tile != null)
            {
                tileinfotext.text = $"Tile Position: ({tile.x:F2}, {tile.z:F2})"; // converting the float to a string
            }
        }
        else
        {
            tileinfotext.text = "";
        }
    }

}
