using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnManager))]
public class SpawnManagerEditor : Editor
{
    SpawnManager spawnManager;

    private void OnEnable()
    {
        spawnManager = (SpawnManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Add Spawner"))
        {
            spawnManager.CreateSpawner();
        }
    }

    private void OnSceneGUI()
    {
        SpawnManager spawnManager = (SpawnManager)target;

        spawnManager.position1 = Handles.PositionHandle(spawnManager.position1, Quaternion.identity);
        spawnManager.position2 = Handles.PositionHandle(spawnManager.position2, Quaternion.identity);
    }
}
