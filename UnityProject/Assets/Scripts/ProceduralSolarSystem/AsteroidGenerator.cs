using UnityEngine;
using LibNoise.Unity;

public class AsteroidGenerator : MonoBehaviour {
    public GameObject asteroidPrefab;
    public int maxAsteroids;
    public float maxRadius;
    public float exclusionRadius;
    public float noiseScale;
    public int octaves;
    public float frequency;
    public float persistance;
    public float lacunarity;
    public int seed;

    public void Generate()
    {
        LibNoise.Unity.Generator.Perlin perlin = new LibNoise.Unity.Generator.Perlin(1f, lacunarity, persistance, octaves, seed, QualityMode.Medium);
        Random.seed = seed;

        for (int i = 0; i < maxAsteroids; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * maxRadius;
            if (randomPos.magnitude > exclusionRadius)
            {
                GameObject asteroid = Instantiate(asteroidPrefab);
                asteroid.transform.parent = transform;
                asteroid.transform.position = randomPos;
                CelestialBody asteroidBody = asteroid.GetComponent<CelestialBody>();
                ProceduralCubeBody proceduralBody = asteroidBody.GetComponent<ProceduralCubeBody>();
                proceduralBody.Prepare();
                proceduralBody.Generate();
                proceduralBody.Finish();
            }
        }
    }
}
