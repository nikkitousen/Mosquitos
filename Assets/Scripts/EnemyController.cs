using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float fallingSpeed;
	
	// References
	private Camera cam;
	private GameObject spawner;
	
	void Awake () {
		cam = Camera.main;
		spawner = GameObject.Find("EnemySpawner");
	}
	
	// Update is called once per frame
	void Update () {
		
		// Update enemy's position
		Vector2 newPos = transform.position;
		newPos.y -= fallingSpeed * Time.deltaTime;
		
		// If the enemy is far below the camera, destroy it
		float cameraMinY = cam.transform.position.y - cam.orthographicSize;
		if(newPos.y < cameraMinY - 2f) {
			spawner.GetComponent<SpawnEnemies>().AddToPool(transform);
		} else {
			transform.position = newPos;
		}
	}
	
}
