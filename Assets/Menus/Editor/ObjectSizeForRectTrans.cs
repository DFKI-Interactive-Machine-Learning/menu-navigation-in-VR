// using UnityEngine;
// using UnityEditor;
// using System.Reflection;
// using System;

// [CustomEditor(typeof(RectTransform))]
// public class ObjectSizeForRectTrans : Editor
// {

//     //Unity's built-in editor
// 	Editor defaultEditor;
// 	Transform _transform;

// 	void OnEnable()
// 	{
        
// 		//When this inspector is created, also create the built-in inspector
// 		defaultEditor = Editor.CreateEditor(targets, Type.GetType("UnityEditor.RectTransformInspector, UnityEditor"));
// 		_transform = target as RectTransform;
// 	}

// 	void OnDisable()
// 	{
// 		//When OnDisable is called, the default editor we created should be destroyed to avoid memory leakage.
// 		//Also, make sure to call any required methods like OnDisable
// 		// MethodInfo disableMethod = defaultEditor.GetType().GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
// 		// disableMethod?.Invoke(defaultEditor, null);
// 		DestroyImmediate(defaultEditor);
// 	}

//     void OnSceneGUI()
//     {
//         Transform transform = (Transform)target;

//         // Get the object's world size
//         Vector3 worldSize = GetWorldSize(transform);

//         // Draw labels in the Scene view
//         Handles.Label(transform.position + Vector3.up * 2, 
//                       $"World Size:\nX: {worldSize.x:F2}\nY: {worldSize.y:F2}\nZ: {worldSize.z:F2}");
//     }

//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         Transform transform = (Transform)target;

//         // Get the object's world size
//         Vector3 worldSize = GetWorldSize(transform);

//         // Display the world size in the Inspector
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("World Size", EditorStyles.boldLabel);
//         EditorGUILayout.LabelField("X", worldSize.x.ToString("F2"));
//         EditorGUILayout.LabelField("Y", worldSize.y.ToString("F2"));
//         EditorGUILayout.LabelField("Z", worldSize.z.ToString("F2"));

//         // If it's a RectTransform, display its size as well
//         RectTransform rectTransform = transform as RectTransform;
//         if (rectTransform != null)
//         {
//             Vector2 rectSize = rectTransform.rect.size;
//             EditorGUILayout.Space();
//             EditorGUILayout.LabelField("RectTransform Size", EditorStyles.boldLabel);
//             EditorGUILayout.LabelField("Width", rectSize.x.ToString("F2"));
//             EditorGUILayout.LabelField("Height", rectSize.y.ToString("F2"));
//         }
//     }

//     private Vector3 GetWorldSize(Transform transform)
//     {
//         // Handle RectTransform
//         RectTransform rectTransform = transform as RectTransform;
//         if (rectTransform != null)
//         {
//             Vector3[] worldCorners = new Vector3[4];
//             rectTransform.GetWorldCorners(worldCorners);

//             float width = Vector3.Distance(worldCorners[0], worldCorners[3]);
//             float height = Vector3.Distance(worldCorners[0], worldCorners[1]);
//             float depth = Vector3.Distance(worldCorners[0], worldCorners[2]);

//             return new Vector3(width, height, depth);
//         }

//         // Handle regular Transform with Renderer
//         Renderer renderer = transform.GetComponent<Renderer>();
//         if (renderer != null)
//         {
//             Vector3 size = renderer.bounds.size;
//             return size;
//         }
//         else
//         {
//             // Handle regular Transform without Renderer
//             Vector3 size = transform.lossyScale;
//             return size;
//         }
//     }
// }
