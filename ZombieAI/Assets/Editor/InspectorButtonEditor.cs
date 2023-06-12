using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(testGenerator))] 
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        testGenerator generator = (testGenerator)target;

        if (GUILayout.Button("Regenerate Map"))
        {
            generator.RegenerateMap();
        }
    }
}