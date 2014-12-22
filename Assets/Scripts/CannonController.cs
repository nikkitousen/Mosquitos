using UnityEngine;
using System.Collections.Generic;

public class CannonController : MonoBehaviour {

	public float radius;
	public float shootForce;
	public float bulletOffset;
	public float handleReturnSpeed;
	public GameObject bulletPrefab;
	
	// Flags
	private bool readyToShoot = false;
	private bool handleReturning = false;
	
	// Pool
	private Stack<GameObject> bulletPool = new Stack<GameObject>();
	
	// References
	private Transform handle;
	private Transform handleGrabPoint;
	
	void Awake() {
		handle = transform.Find ("CannonHandle");
		handleGrabPoint = transform.Find ("HandleGrabPoint");
	}
	
	void Update () {
		// Get the current position of the mouse, relative to the coordinate system
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		
		// Adjust the position of the cannon
		drawCannon(mousePos);	
			
		if(Input.GetMouseButtonDown(0)) {
			// If user pressed close enough to the center of the cannon, remember it
			if(Vector3.Distance(mousePos, handleGrabPoint.position) <= radius) {
				readyToShoot = true;
				handleReturning = false; // If handle was going back, cancel it
			}
		} else if (Input.GetMouseButtonUp(0) && readyToShoot) {
			// If user released and had already pressed in a valid position, shoot
			
			// First, calculate the force of the shot
			Vector3 force = (-1f * handle.position) * shootForce;
			
			// Then, instantiate a bullet prefab, and apply the force
			Vector3 bulletPos = calculateBulletPos();
			GameObject newBullet;
			if(bulletPool.Count == 0) {
				newBullet = Instantiate(bulletPrefab) as GameObject;
			} else {
				newBullet = bulletPool.Pop();
			}
			newBullet.GetComponent<BulletController>().ShootBullet(bulletPos, force);
			
			// Reset flag
			readyToShoot = false;
			
			// Handle now is returning
			handleReturning = true;
		}
		
		// If the handle is returning to the original position, move it accordingly
		if(handleReturning) {
			handle.position = Vector3.Lerp(handle.position,
									       Vector3.zero,
									   	   handleReturnSpeed * Time.deltaTime);
		}
		
	}
	
	// Manages the rotation of the cannon's sprite
	void drawCannon(Vector3 mousePos) {
		if(!readyToShoot) return; // Do not change if not pressed
		
		// Get the angle between the X axis and the position of the mouse
		float angle = Vector3.Angle(Vector3.right, mousePos);
		if(mousePos.y < 0) angle *= -1f;
		
		// Adjust the angle wrt the cannon's sprite
		angle += 90;
		
		// We only allow the cannon to move between -45 and 45 degrees
		if(angle < -45f || angle > 45f) return;
		transform.eulerAngles = new Vector3(0, 0, angle);
		
		// We only allow the handle to move below its original position, and
		// up to a maximum distance
		if (mousePos.sqrMagnitude >= handleGrabPoint.position.sqrMagnitude) {
			float handleNewYPos = Vector3.Distance(mousePos, handleGrabPoint.position);
			if(handleNewYPos <= 1.2)
				handle.localPosition = new Vector3(0, -handleNewYPos, 0);
		}
		
		
	}
	
	// Calculates the starting position of the bullet for the current
	// rotation of the cannon
	Vector3 calculateBulletPos() {
		
		// Get the cannon's rotation angle w.r.t. the X axis, in radians 
		float angle = (transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
		
		return new Vector3(bulletOffset * Mathf.Cos(angle), 
		                   bulletOffset * Mathf.Sin(angle),
		                   0);
	}
	
	public void AddBulletToPool(GameObject bullet) {
		bulletPool.Push(bullet);
	}
}
