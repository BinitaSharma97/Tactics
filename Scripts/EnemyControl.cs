using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// enemy Interface
public interface InterfaceAI
{
    void Initialize(Transform player, GridObstacleData obstacleData);
    void Act(); // called when the enemy should act
}

// Enemy Control
public class EnemyControl : MonoBehaviour, InterfaceAI
{
    [Header("Settings")]
    public float moveSpeed = 3f;
    public float step = 1f;

    [Header("References")]
    public GridObstacleData obstacleData;

    private Transform player;
    private Vector2Int lastPlayerPos;
    private bool isMoving = false;

    // Initiating enemy with player reference and grid obstacle data
    public void Initialize(Transform playerTransform, GridObstacleData data)
    {
        player = playerTransform;
        obstacleData = data;
        lastPlayerPos = GetGridPos(player.position);
    }

    void Start()
    {
        if (player == null) //find player
        {
            PlayerControl playerControl = FindObjectOfType<PlayerControl>();
            if (playerControl != null)
                Initialize(playerControl.transform, obstacleData);
        }
    }

    void Update()
    {
        if (isMoving || player == null) return;

        Vector2Int currentPlayerPos = GetGridPos(player.position);
        if (currentPlayerPos != lastPlayerPos)
        {
            lastPlayerPos = currentPlayerPos;
            Act(); // move only when player moves
        }
    }

    // enemy interface method
    public void Act()
    {
        Vector2Int enemyPos = GetGridPos(transform.position);
        Vector2Int target = FindAdjacentTarget(lastPlayerPos);

        if (target != enemyPos)
        {
            var path = FindPath(enemyPos, target);
            if (path != null) StartCoroutine(MovePath(path)); //move towards player
        }
    }

    Vector2Int GetGridPos(Vector3 pos) =>
        new Vector2Int(Mathf.RoundToInt(pos.x / step), Mathf.RoundToInt(pos.z / step));

    Vector2Int FindAdjacentTarget(Vector2Int playerPos)
    {
        Vector2Int[] options = {
            playerPos + Vector2Int.up,
            playerPos + Vector2Int.down,
            playerPos + Vector2Int.left,
            playerPos + Vector2Int.right
        };

        foreach (var option in options)
        {
            if (IsValid(option) && !obstacleData.IsBlocked(option.x, option.y))
                return option;
        }

        return GetGridPos(transform.position); // enemy fallback
    }

    bool IsValid(Vector2Int pos) =>
        pos.x >= 0 && pos.y >= 0 && pos.x < obstacleData.gridSize && pos.y < obstacleData.gridSize;

    // enemy pathfinding
    List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        var open = new List<Vector2Int> { start };
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var g = new Dictionary<Vector2Int, int> { [start] = 0 };

        while (open.Count > 0)
        {
            Vector2Int current = open[0];
            foreach (var n in open)
                if (Score(n, end, g) < Score(current, end, g)) current = n;

            if (current == end) return Reconstruct(cameFrom, current);

            open.Remove(current);
            foreach (var neighbor in Neighbors(current))
            {
                if (obstacleData.IsBlocked(neighbor.x, neighbor.y)) continue;

                int newCost = g[current] + 1;
                if (!g.ContainsKey(neighbor) || newCost < g[neighbor])
                {
                    cameFrom[neighbor] = current;
                    g[neighbor] = newCost;
                    if (!open.Contains(neighbor)) open.Add(neighbor);
                }
            }
        }
        return null;
    }

    int Score(Vector2Int node, Vector2Int goal, Dictionary<Vector2Int, int> g) =>
        g[node] + Mathf.Abs(node.x - goal.x) + Mathf.Abs(node.y - goal.y);

    List<Vector2Int> Neighbors(Vector2Int p)
    {
        var dirs = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        var list = new List<Vector2Int>();
        foreach (var d in dirs)
        {
            var n = p + d;
            if (IsValid(n)) list.Add(n);
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

    IEnumerator MovePath(List<Vector2Int> path)
    {
        isMoving = true;
        foreach (var p in path)
        {
            Vector3 target = new Vector3(p.x * step, 0.5f, p.y * step);
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        isMoving = false;
    }
}