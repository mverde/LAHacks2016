using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AsteroidGenerator body = (AsteroidGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            body.Generate();
        }
    }
}
