using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CRLevelData))]
public class CRLevelDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CRLevelData levelData = (CRLevelData)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Validate Level Data"))
        {
            CRLevelValidator.ValidateLevel(levelData);
        }
        
        EditorGUILayout.HelpBox("Use 'Validate Level Data' to ensure no overlapping elements, missing IDs, or out-of-bounds positions.", MessageType.Info);
    }
}
