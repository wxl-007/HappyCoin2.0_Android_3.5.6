using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(tk2dCamera))]
public class tk2dCameraEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();

		tk2dCamera _target = (tk2dCamera)target;
		var frameBorderStyle = EditorStyles.textField;
		
		// sanity
		if (_target.resolutionOverride == null)
		{
			_target.resolutionOverride = new tk2dCameraResolutionOverride[0];
			GUI.changed = true;
		}
		
		_target.enableResolutionOverrides = EditorGUILayout.Toggle("Resolution overrides", _target.enableResolutionOverrides);
		if (_target.enableResolutionOverrides)
		{
			EditorGUILayout.LabelField("Native resolution", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			_target.nativeResolutionWidth = EditorGUILayout.IntField("Width", _target.nativeResolutionWidth);
			_target.nativeResolutionHeight = EditorGUILayout.IntField("Height", _target.nativeResolutionHeight);
			EditorGUI.indentLevel--;

			// Overrides
			EditorGUILayout.LabelField("Overrides", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			
			int deleteId = -1;
			for (int i = 0; i < _target.resolutionOverride.Length; ++i)
			{
				var ovr = _target.resolutionOverride[i];
				EditorGUILayout.BeginVertical(frameBorderStyle);
				GUILayout.Space(8);
				ovr.name = EditorGUILayout.TextField("Name", ovr.name);
				ovr.width = EditorGUILayout.IntField("Width", ovr.width);
				ovr.height = EditorGUILayout.IntField("Height", ovr.height);
				ovr.autoScaleMode = (tk2dCameraResolutionOverride.AutoScaleMode)EditorGUILayout.EnumPopup("Auto Scale", ovr.autoScaleMode);
				if (ovr.autoScaleMode == tk2dCameraResolutionOverride.AutoScaleMode.None)
				{
					EditorGUI.indentLevel++;
					ovr.scale = EditorGUILayout.FloatField("Scale", ovr.scale);
					EditorGUI.indentLevel--;
				}
				ovr.fitMode = (tk2dCameraResolutionOverride.FitMode)EditorGUILayout.EnumPopup("Fit Mode", ovr.fitMode);
				if (ovr.fitMode == tk2dCameraResolutionOverride.FitMode.Constant)
				{
					EditorGUI.indentLevel++;
					ovr.offsetPixels.x = EditorGUILayout.FloatField("X", ovr.offsetPixels.x);
					ovr.offsetPixels.y = EditorGUILayout.FloatField("Y", ovr.offsetPixels.y);
					EditorGUI.indentLevel--;
				}
				GUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(" ");
				if (GUILayout.Button("Delete", EditorStyles.miniButton))
					deleteId = i;
				GUILayout.EndHorizontal();
				GUILayout.Space(4);
				EditorGUILayout.EndVertical();
			}
			
			if (deleteId != -1)
			{
				List<tk2dCameraResolutionOverride> ovr = new List<tk2dCameraResolutionOverride>(_target.resolutionOverride);
				ovr.RemoveAt(deleteId);
				_target.resolutionOverride = ovr.ToArray();
				GUI.changed = true;
				Repaint();
			}
			
			EditorGUILayout.BeginVertical(frameBorderStyle);
			GUILayout.Space(32);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Add override", GUILayout.ExpandWidth(false)))
			{
				tk2dCameraResolutionOverride ovr = new tk2dCameraResolutionOverride();
				ovr.name = "Wildcard Override";
				ovr.width = -1;
				ovr.height = -1;
				ovr.autoScaleMode = tk2dCameraResolutionOverride.AutoScaleMode.FitVisible;
				ovr.fitMode = tk2dCameraResolutionOverride.FitMode.Center;
				System.Array.Resize(ref _target.resolutionOverride, _target.resolutionOverride.Length + 1);
				_target.resolutionOverride[_target.resolutionOverride.Length - 1] = ovr;
				GUI.changed = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(32);
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Space();
		
		
		EditorGUILayout.LabelField("Camera resolution", EditorStyles.boldLabel);
		GUIContent toggleLabel = new GUIContent("Force Editor Resolution", 
			"When enabled, forces the resolution in the editor regardless of the size of the game window.");
		EditorGUI.indentLevel++;
		_target.forceResolutionInEditor = EditorGUILayout.Toggle(toggleLabel, _target.forceResolutionInEditor);
		if (_target.forceResolutionInEditor)
		{
			_target.forceResolution.x = EditorGUILayout.IntField("Width", (int)_target.forceResolution.x);
			_target.forceResolution.y = EditorGUILayout.IntField("Height", (int)_target.forceResolution.y);
		}
		else
		{
			EditorGUILayout.FloatField("Width", _target.resolution.x);
			EditorGUILayout.FloatField("Height", _target.resolution.y);
		}
		EditorGUI.indentLevel--;
		
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			tk2dCameraAnchor[] allAlignmentObjects = GameObject.FindObjectsOfType(typeof(tk2dCameraAnchor)) as tk2dCameraAnchor[];
			foreach (var v in allAlignmentObjects)
			{
				EditorUtility.SetDirty(v);
			}
		}
		
		GUILayout.Space(16.0f);
		
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		if (GUILayout.Button("Create Anchor", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
		{
			tk2dCamera cam = (tk2dCamera)target;
			
			GameObject go = new GameObject("Anchor");
			go.transform.parent = cam.transform;
			tk2dCameraAnchor cameraAnchor = go.AddComponent<tk2dCameraAnchor>();
			cameraAnchor.tk2dCamera = cam;
			cameraAnchor.mainCamera = cam.mainCamera;
			
			EditorGUIUtility.PingObject(go);
		}
		
		EditorGUILayout.EndHorizontal();
	}
	
    [MenuItem("GameObject/Create Other/tk2d/Camera", false, 14905)]
    static void DoCreateCameraObject()
	{
		GameObject go = tk2dEditorUtility.CreateGameObjectInScene("tk2dCamera");
		go.transform.position = new Vector3(0, 0, -10.0f);
		Camera camera = go.AddComponent<Camera>();
		camera.orthographic = true;
		camera.orthographicSize = 480.0f; // arbitrary large number
		camera.farClipPlane = 1000.0f;
		go.AddComponent<tk2dCamera>();

		Selection.activeGameObject = go;
		Undo.RegisterCreatedObjectUndo(go, "Create tk2dCamera");
	}
}
