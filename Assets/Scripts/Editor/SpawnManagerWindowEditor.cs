using UnityEngine;
using UnityEditor;

public class SpawnManagerWindowEditor : EditorWindow
{
    [Header("Prefabs")]
    GameObject enemyPrefab;

    [Header("Spawner coordinates")]
    Vector3 position1;
    Vector3 position2;
    Vector3 position1Def = new Vector3(-5,0,0);
    Vector3 position2Def = new Vector3(5, 0, 0);

    [Header("Time between spawns")]
    float spawnRate;

    [Header("Number of enemies")]
    int enemyCount = 1;

    [Header("Is the spawner working ?")]
    bool isSpawning;

    bool displayHandles;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/HulaOhNo/Spawner manager")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SpawnManagerWindowEditor window = (SpawnManagerWindowEditor)EditorWindow.GetWindow(typeof(SpawnManagerWindowEditor),false, "Spawn manager");
        window.Show();
    }

    private void OnEnable()
    {
        displayHandles = true;
        ResetAll();
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    public void OnGUI()
    {
        GUILayout.Label("Anchor position", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        position1 = EditorGUILayout.Vector3Field("Position 1", position1);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            position1 = position1Def;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        position2 = EditorGUILayout.Vector3Field("Position 2", position2);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            position1 = position1Def;
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("How many seconds between spawns/how many spawns", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        spawnRate = EditorGUILayout.FloatField("Spawn rate",spawnRate);
        enemyCount = EditorGUILayout.IntField("Number of enemies", enemyCount);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        isSpawning = EditorGUILayout.Toggle("Is the spawner working", isSpawning);
        enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy prefab", enemyPrefab, typeof(GameObject), true);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();



        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add a spawner", GUILayout.Height(40)))
            CreateSpawner();
        if (GUILayout.Button("Reset all", GUILayout.Height(40)))
            ResetAll();
        GUILayout.EndHorizontal();

        displayHandles = EditorGUILayout.Toggle("Display handles", displayHandles);
    }

    void OnSceneGUI(SceneView sceneView)
    {
        Handles.color = Color.magenta;
        Handles.DrawSphere(0,position1,Quaternion.identity,0.3f);
        Handles.DrawSphere(0, position2, Quaternion.identity, 0.3f);
        Handles.DrawLine(position1, position2);

        if (displayHandles)
        {
            position1 = Handles.PositionHandle(position1, Quaternion.identity);
            position2 = Handles.PositionHandle(position2, Quaternion.identity);
        }
    }

    void CreateSpawner()
    {
        GameObject spawner = new GameObject("Spawner");
        EnemySpawner spawnerScript = spawner.AddComponent<EnemySpawner>();
        spawnerScript.enemyPrefab = enemyPrefab;
        spawnerScript.enemyCount = enemyCount;
        spawnerScript.spawnRate = spawnRate;
        spawnerScript.position1 = position1;
        spawnerScript.position2 = position2;
        spawnerScript.isSpawning = isSpawning;
    }

    void ResetAll()
    {
        position1 = position1Def;
        position2 = position2Def;
        enemyCount = 1;
        spawnRate = 1;
        isSpawning = true;
    }
}
