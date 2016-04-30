using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Shooter))]
public class ShooterInspector : Editor
{
	public override void OnInspectorGUI()
	{
		Shooter shooter = (Shooter)target;

		DrawDefaultInspector ();

		if (GUILayout.Button("Shoot"))
		{
			shooter.Shoot ();
		}
	}
}
