using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rito.JobTest;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainTester : Editor {
    public TerrainGenerator main;
    private void Awake() {
        main = (TerrainGenerator)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Basic")) {
            main.StartAt = Time.realtimeSinceStartup;
            main.TestBasic();
        }

        if (GUILayout.Button("Coroutine")) {
            main.TestCoroutine();
        }
        if (GUILayout.Button("Job Sync")) {
            main.StartAt = Time.realtimeSinceStartup;
            main.TestJobSync();
        }
        if (GUILayout.Button("Job Async")) {
            main.StartAt = Time.realtimeSinceStartup;
            main.TestJobAsync();
        }

        if (GUILayout.Button("Clear")) {
            main.StopAllCoroutines();
            main.ClearObjects();
        }
    }
}
