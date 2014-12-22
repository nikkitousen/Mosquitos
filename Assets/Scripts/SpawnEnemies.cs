using UnityEngine;
using System.Collections.Generic;

public class SpawnEnemies : MonoBehaviour {

	public GameObject mosquitoPrefab;
	public float minDelay;
	public float maxDelay;
	
	// Pool
	private Stack<Transform> EnemyPool = new Stack<Transform>();
	
	// References
	private Camera cam;
	
	// Use this for initialization
	void Start () {
		Invoke("SpawnMosquito", Random.Range(minDelay, maxDelay));
	}
	
	void Awake() {
		cam = Camera.main;
	}
	
	void SpawnMosquito() {
	
		// Get min and max X coordinates in the camera, and also the max Y coordinate 
		float minX = -1f * cam.aspect * cam.orthographicSize;
		float maxX = cam.aspect * cam.orthographicSize;
		float maxY = cam.orthographicSize + cam.transform.position.y;
		
		// The new mosquito will spawn 1 unit above the camera, and in a
		// random X position
		Vector3 spawnPosition = new Vector3(Random.Range(minX+0.5f, maxX-0.5f),
										    maxY + 1f,
										    0);
		
		Transform newMosquito;
		if(EnemyPool.Count == 0) {
			newMosquito = (Instantiate(mosquitoPrefab,
						 			   spawnPosition,
									   Quaternion.identity) as GameObject)
									   .transform;
		} else {
			newMosquito = EnemyPool.Pop();
			newMosquito.position = spawnPosition;
		}
		
		// If the new mosquito appears in the left half of the screen, flip it
		if (spawnPosition.x < 0) 
			newMosquito.eulerAngles = new Vector3(0, 180, 0);
		
		// Spawn a new mosquito
		Invoke("SpawnMosquito", Random.Range(minDelay, maxDelay));
	}
	
	public void AddToPool(Transform t) {
		EnemyPool.Push(t);
	}
	
}
