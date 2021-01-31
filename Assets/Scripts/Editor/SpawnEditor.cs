using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class SpawnEditor : Editor
{
    EnemySpawner enemySpawner;
    Tool LastTool = Tool.None;

    private void OnEnable()
    {
        enemySpawner = (EnemySpawner)target;

        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }

    private void OnSceneGUI()
    {
        enemySpawner.position1 = Handles.PositionHandle(enemySpawner.position1, Quaternion.identity);
        enemySpawner.position2 = Handles.PositionHandle(enemySpawner.position2, Quaternion.identity);

        enemySpawner.transform.position = Vector3.Lerp(enemySpawner.position1, enemySpawner.position2, 0.5f);
    }
}
