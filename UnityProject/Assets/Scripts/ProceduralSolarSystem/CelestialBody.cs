using UnityEngine;

[RequireComponent(typeof(ProceduralCubeBody))]
public class CelestialBody : MonoBehaviour {
    public float rotationSpeed = 1f;
    public Biome biome;

    private void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
    }
}
