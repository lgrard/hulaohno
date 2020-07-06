using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraCPWindowEditor : EditorWindow
{
    int numberOfSpawner;
    EnemySpawner spawnerToLink_1;
    EnemySpawner spawnerToLink_2;
    EnemySpawner spawnerToLink_3;
    EnemySpawner spawnerToLink_4;

    Vector3 cameraPosition;
    Vector3 cameraRotation;
    Vector3 cameraPositionDef = new Vector3(0,4,-11);
    Vector3 cameraRotationDef = new Vector3(9,0,0);

    Vector3 zonePosition;
    Vector3 zoneSize;
    Vector3 zonePositionDef = new Vector3(0,2.5f,0);
    Vector3 zoneSizeDef = new Vector3 (30,5,25);

    bool displayHandles;
    bool blocksPlayers;


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/HulaOhNo/Checkpoint manager")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CameraCPWindowEditor window = (CameraCPWindowEditor)EditorWindow.GetWindow(typeof(CameraCPWindowEditor),false, "Checkpoint manager");
        window.Show();
    }

    private void OnEnable()
    {
        displayHandles = true;
        ResetAll();
    }

    void OnGUI()
    {
        GUILayout.Label("Camera settings", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        cameraPosition = EditorGUILayout.Vector3Field("Camera Position", cameraPosition);
            if(GUILayout.Button("Reset",GUILayout.Height(40)))
                cameraPosition = cameraPositionDef;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        cameraRotation = EditorGUILayout.Vector3Field("Camera Rotation", cameraRotation);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            cameraRotation = cameraRotationDef;
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("Trigger zone settings", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        zonePosition = EditorGUILayout.Vector3Field("Zone position", zonePosition);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            zonePosition = zonePositionDef;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        zoneSize = EditorGUILayout.Vector3Field("Zone size", zoneSize);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            zoneSize = zoneSizeDef;
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        blocksPlayers = EditorGUILayout.Toggle("Does this block the player", blocksPlayers);
        if (blocksPlayers)
        {
            numberOfSpawner = EditorGUILayout.IntSlider("Number of spawner",numberOfSpawner, 0, 4);
            if (numberOfSpawner > 0)
                spawnerToLink_1 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 1", spawnerToLink_1, typeof(EnemySpawner), true);
            if (numberOfSpawner > 1)
                spawnerToLink_2 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 2", spawnerToLink_2, typeof(EnemySpawner), true);
            if (numberOfSpawner > 2)
                spawnerToLink_3 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 3", spawnerToLink_3, typeof(EnemySpawner), true);
            if (numberOfSpawner > 3)
                spawnerToLink_4 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 4", spawnerToLink_4, typeof(EnemySpawner), true);
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Add a camera checkpoint", GUILayout.Height(40)))
            AddCameraCP();
        if (GUILayout.Button("Reset all", GUILayout.Height(40)))
            ResetAll();
        GUILayout.EndHorizontal();

        displayHandles = EditorGUILayout.Toggle("Display handles", displayHandles);
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (displayHandles)
        {
            cameraPosition = Handles.PositionHandle(cameraPosition, Quaternion.Euler(cameraRotation));
            zonePosition = Handles.PositionHandle(zonePosition, Quaternion.identity);
            zoneSize = Handles.ScaleHandle(zoneSize, zonePosition, Quaternion.identity, 2);
        }

        Handles.color = Color.green;
        Handles.DrawCone(0, cameraPosition, Quaternion.Euler(cameraRotation), 2);
        Handles.DrawWireCube(zonePosition, zoneSize);
    }

    void ResetAll()
    {
        cameraPosition = cameraPositionDef;
        cameraRotation = cameraRotationDef;
        zonePosition = zonePositionDef;
        zoneSize = zoneSizeDef;
        spawnerToLink_1 = null;
        spawnerToLink_2 = null;
        spawnerToLink_3 = null;
        spawnerToLink_4 = null;
    }

    void AddCameraCP()
    {
        GameObject camCp = new GameObject("CamCP");
        CameraCheckPoint camCpScript = camCp.AddComponent<CameraCheckPoint>();
        camCpScript.cameraPosition = cameraPosition;
        camCpScript.cameraRotation = cameraRotation;
        camCpScript.zonePosition = zonePosition;
        camCpScript.zoneSize = zoneSize;
        camCpScript.blocksPlayers = blocksPlayers;

        if (blocksPlayers)
        {
            BoxCollider boundL = camCp.AddComponent<BoxCollider>();
            BoxCollider boundR = camCp.AddComponent<BoxCollider>();
            camCpScript.boundL = boundL;
            camCpScript.boundR = boundR;
            camCpScript.boundL.enabled = false;
            camCpScript.boundR.enabled = false;

            camCpScript.linkedSpawner = new List<EnemySpawner>();
            if (numberOfSpawner > 0 && spawnerToLink_1 !=null)
                camCpScript.linkedSpawner.Add(spawnerToLink_1);
            if (numberOfSpawner > 1 && spawnerToLink_2 != null)
                camCpScript.linkedSpawner.Add(spawnerToLink_2);
            if (numberOfSpawner > 2 && spawnerToLink_3 != null)
                camCpScript.linkedSpawner.Add(spawnerToLink_3);
            if (numberOfSpawner > 3 && spawnerToLink_4 != null)
                camCpScript.linkedSpawner.Add(spawnerToLink_4);
        }
    }
}

