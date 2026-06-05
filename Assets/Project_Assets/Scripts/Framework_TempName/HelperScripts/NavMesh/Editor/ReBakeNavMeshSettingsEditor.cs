using UnityEditor;
using UnityEngine;

namespace Project_Assets.Scripts.Framework_TempName.HelperScripts.NavMesh.Editor
{
    [CustomEditor(typeof(ReBakeNavMeshSettings))]
    public class ReBakeNavMeshSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(8);

            if (GUILayout.Button("ReBake"))
            {
                var settings = (ReBakeNavMeshSettings)target;
                settings.ReBake();

                EditorUtility.SetDirty(settings);
            }
        }
    }
}
