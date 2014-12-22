using UnityEngine;
using System.Collections.Generic;

public class BulletController : MonoBehaviour {
	
	public float rotationSpeed;
	
	// Flags
	private bool onPool = false;
	
	// References
	private Camera cam;
	private GameObject cannon;
	
	
	void Awake() {
		cam = Camera.main;
		cannon = GameObject.Find("Cannon");
	}
	
	void Start () {
		transform.rigidbody2D.AddTorque(rotationSpeed);
	}
	
	void Update () {
		// Get min and max X coordinates in the camera, and also the max Y coordinate 
		float minX = -1f * cam.aspect * cam.orthographicSize;
		float maxX = cam.aspect * cam.orthographicSize;
		float minY = cam.transform.position.y - cam.orthographicSize;
		
		if(!onPool) {
			if(transform.position.x < minX - 1f 
			|| transform.position.x > maxX + 1f 
			|| transform.position.y < minY - 1f) {
				cannon.GetComponent<CannonController>().AddBulletToPool(gameObject);
				onPool = true;
			}
		}
	}
	
	public void ShootBullet(Vector3 startingPos, Vector3 force) {
		transform.position = startingPos;
		rigidbody2D.velocity = Vector3.zero;
		rigidbody2D.AddForce(force);
		onPool = false;
	}
}
