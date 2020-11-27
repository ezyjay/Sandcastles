using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[@CustomEditor(typeof(Transform))]
class TransformEditor : Editor {
	
	public override void OnInspectorGUI()
    {
        Transform t =(Transform)target;
             
        // Replicate the standard transform inspector gui
        if (!EditorGUIUtility.wideMode)
		{
			EditorGUIUtility.wideMode = true;
			EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - 212;
		}	
        EditorGUI.indentLevel = 0;

		//Position, scale, rotation
        Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);	
		EditorGUI.BeginDisabledGroup(true);
        Vector3 worldPosition = EditorGUILayout.Vector3Field("World", t.position);
		EditorGUI.EndDisabledGroup();
        Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);

		//Reset transforms
		GUILayout.Space(5f);
		if (GUILayout.Button("Reset Transforms")) {
			Undo.RegisterCompleteObjectUndo(t,"Reset Transforms "+ t.name);
			t.transform.position = Vector3.zero;
			t.transform.rotation = Quaternion.identity;
			t.transform.localScale = Vector3.one;
		}

        if(GUI.changed)
        {
            Undo.RegisterCompleteObjectUndo(t,"Transform Change");
            t.localPosition = FixIfNaN(position);
            t.localEulerAngles = FixIfNaN(eulerAngles);
            t.localScale = FixIfNaN(scale);
        }
    }
    private Vector3 FixIfNaN(Vector3 v)
    {
        if(float.IsNaN(v.x))
        {
            v.x =0;
        }
        if(float.IsNaN(v.y))
        {
            v.y =0;
        }
        if(float.IsNaN(v.z))
        {
            v.z =0;
        }
        return v;
    }
}
//}
