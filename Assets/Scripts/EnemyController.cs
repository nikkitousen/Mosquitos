using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	
	public float fallingSpeed;
	
	// Auxiliary variables
	private bool onPool = false;
	
	// References
	private Camera cam;
	private GameObject spawner;
	
	void Awake () {
		cam = Camera.main;
		spawner = GameObject.Find("EnemySpawner");
	}
	
	// Update is called once per frame
	void Update () {
		
		if(onPool) return;
		
		// Update enemy's position
		Vector2 newPos = transform.position;
		newPos.y -= fallingSpeed * Time.deltaTime;
		
		// If the enemy is far below the camera, destroy it
		float cameraMinY = cam.transform.position.y - cam.orthographicSize;
		if(newPos.y < cameraMinY - 2f) {
			spawner.GetComponent<SpawnEnemies>().AddToPool(transform);
			onPool = true;
		} else {
			transform.position = newPos;
		}
	}
	
	public void Respawn(Vector3 newPos) {
		onPool = false;
		transform.position = newPos;
		SpriteRenderer[] childrenSprites =  transform.GetComponentsInChildren<SpriteRenderer>();
		for(int i=0; i<childrenSprites.Length; i++) {
			childrenSprites[i].enabled = true;
		}
	}
	
	public void Dissapear() {
		SpriteRenderer[] childrenSprites =  transform.GetComponentsInChildren<SpriteRenderer>();
		for(int i=0; i<childrenSprites.Length; i++) {
			childrenSprites[i].enabled = false;
		}
	}
}
