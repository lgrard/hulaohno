using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraCheckPoint))]

public class CameraCPEditor : Editor
{
    CameraCheckPoint camCP;
    Tool LastTool = Tool.None;
    Vector3 cameraPositionDef = new Vector3(0, 4, -11);
    Vector3 cameraRotationDef = new Vector3(9, 0, 0);
    Vector3 zonePositionDef = new Vector3(0, 2.5f, 0);
    Vector3 zoneSizeDef = new Vector3(30, 5, 25);

    private void OnEnable()
    {
        camCP = (CameraCheckPoint)target;

        LastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable()
    {
        Tools.current = LastTool;
    }

    private void OnSceneGUI()
    {

        camCP.zonePosition = Handles.PositionHandle(camCP.zonePosition, Quaternion.identity);
        camCP.zoneSize = Handles.ScaleHandle(camCP.zoneSize, camCP.zonePosition, Quaternion.identity, 2);

        Handles.color = new Color(0, 180, 0);
        if (camCP.controlsCam)
        {
            camCP.cameraPosition = Handles.PositionHandle(camCP.cameraPosition, Quaternion.Euler(camCP.cameraRotation));
            Handles.DrawCone(0, camCP.cameraPosition, Quaternion.Euler(camCP.cameraRotation), 1.5f);
        }

        Handles.DrawWireCube(camCP.zonePosition, camCP.zoneSize);

        Handles.DrawWireCube(camCP.zonePosition, camCP.triggerSize);
    }

    public override void OnInspectorGUI()
    {
        if (camCP.controlsCam)
        {
            GUILayout.Label("Camera settings", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            camCP.cameraPosition = EditorGUILayout.Vector3Field("Camera Position", camCP.cameraPosition);
            if (GUILayout.Button("Reset", GUILayout.Height(40)))
                camCP.cameraPosition = cameraPositionDef;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            camCP.cameraRotation = EditorGUILayout.Vector3Field("Camera Rotation", camCP.cameraRotation);
            if (GUILayout.Button("Reset", GUILayout.Height(40)))
                camCP.cameraRotation = cameraRotationDef;
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        GUILayout.Label("Trigger zone settings", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        camCP.zonePosition = EditorGUILayout.Vector3Field("Zone position", camCP.zonePosition);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            camCP.zonePosition = zonePositionDef;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        camCP.zoneSize = EditorGUILayout.Vector3Field("Zone size", camCP.zoneSize);
        if (GUILayout.Button("Reset", GUILayout.Height(40)))
            camCP.zoneSize = zoneSizeDef;
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        camCP.triggerOffset = EditorGUILayout.FloatField("Trigger zone offset", camCP.triggerOffset);


        GUILayout.BeginHorizontal();
        camCP.blocksPlayers = EditorGUILayout.Toggle("Does this block the player", camCP.blocksPlayers);
        camCP.controlsCam = EditorGUILayout.Toggle("Does this moves camera", camCP.controlsCam);
        GUILayout.EndHorizontal();

        GUILayout.Label("*Edit the linked spawner in Debug mode", EditorStyles.miniLabel);
    }
}
