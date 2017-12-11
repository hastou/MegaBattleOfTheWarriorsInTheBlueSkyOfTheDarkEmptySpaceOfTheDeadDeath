using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour {

    public List<GameObject> Asteroids;

    public GameObject Teleporter;

    public int NbOfAsteroids;

    public int Dispersion;

    public int DispersionFactor;

    public bool UseSeed;
    
    public int Size;

    public int Speed;

	// Use this for initialization
	void Start () {

        if (UseSeed) Random.InitState(1);

        for (int i = 0; i < NbOfAsteroids; i++)
        {

            int asteroidPrefabIdx = Random.Range(0, Asteroids.Count - 1);

            int x, y, z;
            x = Random.Range(-Dispersion/2, Dispersion/2) * DispersionFactor;
            y = Random.Range(-Dispersion/2, Dispersion/2) * DispersionFactor;
            z = Random.Range(0, Dispersion*5) * DispersionFactor;

            GameObject currentAsteroid = Instantiate(
                // Idx of the prefab (0-7)
                Asteroids[asteroidPrefabIdx], 
                // Random position
                new Vector3(x, y, z),
                // Random rotation
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))
                ) as GameObject;

            float randomSize = Random.Range(Size, Size * 5);
            currentAsteroid.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

            Rigidbody asteroidRigidbody = currentAsteroid.AddComponent<Rigidbody>() as Rigidbody;
            asteroidRigidbody.useGravity = false;
            //asteroidRigidbody.mass = randomSize;
            asteroidRigidbody.AddForce(currentAsteroid.transform.forward * Speed);

            MeshCollider asteroidCollider = currentAsteroid.AddComponent<MeshCollider>() as MeshCollider;
            asteroidCollider.convex = true;
            asteroidCollider.inflateMesh = true;
            asteroidCollider.skinWidth = 0.01f;
        }        
	}
}
