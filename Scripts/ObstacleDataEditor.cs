using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridObstacleData))]
public class ObstacleDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridObstacleData data = (GridObstacleData)target;

        //combining the array and grid size
        if (data.blockedTiles == null || data.blockedTiles.Length != data.gridSize * data.gridSize)
        {
            data.blockedTiles = new bool[data.gridSize * data.gridSize];
            EditorUtility.SetDirty(data);
        }

        for (int y = 0; y < data.gridSize; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < data.gridSize; x++)
            {
                int index = y * data.gridSize + x;
                data.blockedTiles[index] = EditorGUILayout.Toggle(data.blockedTiles[index], GUILayout.Width(20));
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(data); //saving the toggled data on play
        }
    }
}