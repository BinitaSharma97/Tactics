using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GridObstacleData obstacleData;
    public Animator animator; //for animation
    public float step = 1f;
    public float moveSpeed = 3f;

    bool isMoving = false;

    void Update()
    {
        if (isMoving) return;

        if (Input.GetMouseButtonDown(0)) //to make the player go to the clicked position
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TilesInform tile = hit.collider.GetComponent<TilesInform>();
                if (tile != null)
                {
                    Vector2Int start = new Vector2Int(    // Current position of the player
                        Mathf.RoundToInt(transform.position.x / step),
                        Mathf.RoundToInt(transform.position.z / step)
                    );

                    Vector2Int end = new Vector2Int(      // Target grid position (convert world position to grid index)
                        Mathf.RoundToInt(tile.x / step),
                        Mathf.RoundToInt(tile.z / step)
                    );

                    var path = FindPath(start, end);
                    if (path != null) StartCoroutine(MovePath(path));
                }
            }
        }
    }

    // Pathfinding
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        var open = new List<Vector2Int> { start };
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var g = new Dictionary<Vector2Int, int> { [start] = 0 };
        var f = new Dictionary<Vector2Int, int> { [start] = Heuristic(start, end) }; // guessing how far the player is from the goal

        while (open.Count > 0)
        {
            Vector2Int current = open[0];
            foreach (var n in open) if (f[n] < f[current]) current = n;

            if (current == end) return Reconstruct(cameFrom, current);

            open.Remove(current);
            foreach (var neighbor in Neighbors(current))
            {
                if (obstacleData.IsBlocked(neighbor.x, neighbor.y)) continue;

                int tentative = g[current] + 1;
                if (!g.ContainsKey(neighbor) || tentative < g[neighbor])
                {
                    cameFrom[neighbor] = current;
                    g[neighbor] = tentative;
                    f[neighbor] = tentative + Heuristic(neighbor, end);
                    if (!open.Contains(neighbor)) open.Add(neighbor);
                }
            }
        }
        return null;
    }

    int Heuristic(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    List<Vector2Int> Neighbors(Vector2Int node) // neighbors define the tiles around the player
    {
        var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var list = new List<Vector2Int>();
        foreach (var d in dirs)
        {
            var n = node + d;
            if (n.x >= 0 && n.y >= 0 && n.x < obstacleData.gridSize && n.y < obstacleData.gridSize)
                list.Add(n);
        }
        return list;
    } 

    List<Vector2Int> Reconstruct(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        var path = new List<Vector2Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    // Player Movement
    IEnumerator MovePath(List<Vector2Int> path)
    {
        isMoving = true;

        foreach (var p in path)
        {
            Vector3 target = new Vector3(p.x * step, 0.5f, p.y * step);
            Vector3 direction = (target - transform.position).normalized;

            // Choose animation based on direction
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                if (direction.x > 0) animator.Play("Right");
                else animator.Play("Left");
            }
            else
            {
                if (direction.z > 0) animator.Play("Run");
                else animator.Play("Run_Back");
            }

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        animator.Play("Idle"); // back to idle when done
        isMoving = false;

    }
}