using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraCPWindowEditor : EditorWindow
{
    GUIStyle textStyle = new GUIStyle();

    float triggerOffset;
    int numberOfSpawner;
    EnemySpawner spawnerToLink_1;
    EnemySpawner spawnerToLink_2;
    EnemySpawner spawnerToLink_3;
    EnemySpawner spawnerToLink_4;

    GameObject wallPrefab;

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
    bool controlsCam;
    bool spawnEnemies;


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/HulaOhNo/Zone manager")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CameraCPWindowEditor window = (CameraCPWindowEditor)EditorWindow.GetWindow(typeof(CameraCPWindowEditor),false, "Zone manager");
        window.Show();
    }

    private void OnEnable()
    {
        textStyle.normal.textColor = Color.red;
        displayHandles = true;
        ResetAll();
    }

    void OnGUI()
    {
        if (controlsCam)
        {
            GUILayout.Label("Camera settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            cameraPosition = EditorGUILayout.Vector3Field("Camera Position", cameraPosition);
            if (GUILayout.Button("Reset", GUILayout.Height(40)))
                cameraPosition = cameraPositionDef;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            cameraRotation = EditorGUILayout.Vector3Field("Camera Rotation", cameraRotation);
            if (GUILayout.Button("Reset", GUILayout.Height(40)))
                cameraRotation = cameraRotationDef;
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

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

        triggerOffset = EditorGUILayout.FloatField("Trigger zone offset", triggerOffset);
        if(blocksPlayers)
            triggerOffset = Mathf.Clamp(triggerOffset, 6, 10000);

        GUILayout.BeginHorizontal();
        spawnEnemies = EditorGUILayout.Toggle("Does this spawn enemies", spawnEnemies);
        controlsCam = EditorGUILayout.Toggle("Does this moves camera", controlsCam);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

            wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall prefab", wallPrefab, typeof(GameObject), true);

            if (wallPrefab == null)
                GUILayout.Label("You have to assign the wall prefab even if this doesn't block the player", textStyle);
            EditorGUILayout.Space();

        if (spawnEnemies)
        {
            blocksPlayers = EditorGUILayout.Toggle("Does this block the player", blocksPlayers);

            numberOfSpawner = EditorGUILayout.IntSlider("Number of spawner", numberOfSpawner, 1, 4);

            if (numberOfSpawner > 0)
                spawnerToLink_1 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 1", spawnerToLink_1, typeof(EnemySpawner), true);
            if (numberOfSpawner > 1)
                spawnerToLink_2 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 2", spawnerToLink_2, typeof(EnemySpawner), true);
            if (numberOfSpawner > 2)
                spawnerToLink_3 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 3", spawnerToLink_3, typeof(EnemySpawner), true);
            if (numberOfSpawner > 3)
                spawnerToLink_4 = (EnemySpawner)EditorGUILayout.ObjectField("Spawner 4", spawnerToLink_4, typeof(EnemySpawner), true);

            if (numberOfSpawner > 0 && spawnerToLink_1 == null)
                GUILayout.Label("You have to select at least 1 spawner", textStyle);
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
            if(controlsCam)
                cameraPosition = Handles.PositionHandle(cameraPosition, Quaternion.Euler(cameraRotation));

            zonePosition = Handles.PositionHandle(zonePosition, Quaternion.identity);
            zoneSize = Handles.ScaleHandle(zoneSize, zonePosition, Quaternion.identity, 2);
        }

        Handles.color = Color.green;
        if (controlsCam)
            Handles.DrawCone(0, cameraPosition, Quaternion.Euler(cameraRotation), 2);

        Handles.DrawWireCube(zonePosition, zoneSize);
        Handles.DrawWireCube(zonePosition, new Vector3(zoneSize.x-triggerOffset,zoneSize.y,zoneSize.z-triggerOffset));
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
        if (!blocksPlayers || blocksPlayers && wallPrefab != null && spawnerToLink_1 !=null)
        {
            GameObject camCp = new GameObject("CamCP");
            CameraCheckPoint camCpScript = camCp.AddComponent<CameraCheckPoint>();
            camCpScript.cameraPosition = cameraPosition;
            camCpScript.cameraRotation = cameraRotation;
            camCpScript.zonePosition = zonePosition;
            camCpScript.triggerOffset = triggerOffset;
            camCpScript.zoneSize = zoneSize;
            camCpScript.blocksPlayers = blocksPlayers;
            camCpScript.controlsCam = controlsCam;


            Object wallL = PrefabUtility.InstantiatePrefab(wallPrefab,camCp.transform);
            wallL.name = "wallL";

            Object wallR = PrefabUtility.InstantiatePrefab(wallPrefab, camCp.transform);
            wallR.name = "wallR";

            camCpScript.wallL = wallL as GameObject;
            camCpScript.wallR = wallR as GameObject;
            camCpScript.wallL.transform.localScale = new Vector3(1, 10, zoneSize.z);
            camCpScript.wallL.transform.localPosition = new Vector3(zoneSize.x / 2 + camCpScript.wallL.transform.localScale.x / 2, 2, 0);
            camCpScript.wallR.transform.localScale = new Vector3(2, 10, zoneSize.z);
            camCpScript.wallR.transform.localPosition = new Vector3(-zoneSize.x / 2 - camCpScript.wallL.transform.localScale.x / 2, 2, 0);
            camCpScript.wallL.SetActive(false);
            camCpScript.wallR.SetActive(false);

            camCpScript.linkedSpawner = new List<EnemySpawner>();
            
            if (spawnEnemies)
            {
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
}

