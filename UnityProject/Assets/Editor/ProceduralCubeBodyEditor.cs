using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralCubeBody))]
public class ProceduralCubeBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ProceduralCubeBody body = (ProceduralCubeBody)target;

        if (DrawDefaultInspector())
        {
            if (body.parentBody.biome.terrainGenData.autoUpdate)
            {
                body.Prepare();
                body.Generate();
                body.Finish();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            body.Prepare();
            body.Generate();
            body.Finish();
        }
    }
}
