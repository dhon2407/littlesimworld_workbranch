﻿#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using UnityEditor;

public static class HelperEditorUtilities {

	[MenuItem("GameObject/Custom/Create Group %g")]
	public static void CreateGroup() {
		var selected = Selection.gameObjects.ToList();
		if (selected == null || selected.Count == 0) { return; }

		var groupGO = new GameObject("Group");
		Undo.RegisterCreatedObjectUndo(groupGO, "Created New Group");

		foreach (var go in selected) {
			var goParent = go.transform.parent?.gameObject;
			bool markSkipped = false;
			while (goParent != null) {
				if (selected.Contains(goParent)) { markSkipped = true; break; }
				goParent = goParent.transform.parent?.gameObject;
			}
			if (markSkipped) { continue; }
			Undo.SetTransformParent(go.transform, groupGO.transform, $"{go} is now a group child");
			EditorUtility.SetDirty(go);
		}

		EditorUtility.SetDirty(groupGO);
		Selection.activeObject = groupGO;
		Undo.FlushUndoRecordObjects();
	}

	[MenuItem("GameObject/Custom/Select Player %q")]
	public static void SelectPlayer() {

		var player = GameObject.Find("Player");
		Selection.activeGameObject = player;

		var view = SceneView.lastActiveSceneView;
		var offset = view.pivot - view.camera.transform.position;
		var cameraDistance = offset.magnitude;
		view.pivot = player.transform.position + Vector3.forward * cameraDistance * -1.0f;
	}

	//[MenuItem("GameObject/Custom/Edit Collider %e")]
	//public static void EditCollider(GameObject obj) {
	//	if (m_polygonUtility == null) {
	//		System.Type type = Assembly.Load("UnityEditor").GetType("UnityEditor.Collider2DEditorBase");
	//		var ctors = type.GetConstructors();
	//		var m_polygonUtility = ctors[0].Invoke(new object[] { });
	//		var startEditing = type.GetMethod("StartEditing");
	//		PolygonCollider2D collider = obj.GetComponent<PolygonCollider2D>();
	//		startEditing.Invoke(m_polygonUtility, new object[] { collider });
	//
	//		ChangeEditMode(toggleEnabled ? mode : SceneViewEditMode.None, getBoundsOfTargets == null ? owner.GetWorldBoundsOfTargets()
	//
	//
	//	}
	//
	//	if (collider != null) {
	//	}
	//}

	[MenuItem("Edit/Custom/Edit Collider Mode %e")]
	private static void EditCollider() {
		bool shouldReturn = Selection.gameObjects.Length != 1 || Selection.activeGameObject == null || Selection.activeGameObject.scene == null;
		if (shouldReturn) { return; }
		var collider = Selection.activeGameObject.GetComponent<Collider2D>();
		if (collider == null) { return; }



		if (UnityEditorInternal.EditMode.editMode == UnityEditorInternal.EditMode.SceneViewEditMode.Collider) {
			UnityEditorInternal.EditMode.ChangeEditMode(UnityEditorInternal.EditMode.SceneViewEditMode.None, new Bounds(), null);
		}
		else {
			var type = System.Type.GetType("UnityEditor." + collider.GetType().Name + "Editor,UnityEditor.dll");
			if (type == null) { return; }

			var editor = (Editor) Resources.FindObjectsOfTypeAll(type).First();
			if (editor == null) { return; }

			UnityEditorInternal.EditMode.ChangeEditMode(UnityEditorInternal.EditMode.SceneViewEditMode.Collider, new Bounds(SceneView.lastActiveSceneView.camera.transform.position, Vector3.zero), editor);
		}
	}
}
#endif