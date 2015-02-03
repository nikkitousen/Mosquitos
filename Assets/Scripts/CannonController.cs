using UnityEngine;
using System.Collections.Generic;

public class CannonController : MonoBehaviour {

	public float grabRadius;
	public float shootForce;
	public float bulletOffset;
	public float handleReturnSpeed;
	public GameObject bulletPrefab;
	public float epsilonTimeForShoot;
	
	private float lastTimeAtStartPoint = 0f;
	private float lastTimeAtEndPoint = 0f;
	
	// Flags
	private bool readyToShoot = false;
	private bool handleIsGrabbed = false;
	
	// Pool
	private Stack<GameObject> bulletPool = new Stack<GameObject>();
	
	// References
	private Transform handle;
	private Transform handleGrabPoint;
	private Transform handleInitialGrabPoint;
	private Transform handleMinPos;
	private Transform handleMaxPos;
	private Transform handleShootStartPoint;
	private Transform handleShootEndPoint;
	
	void Awake() {
		handle = transform.Find ("Handle");
		handleGrabPoint = GameObject.Find ("HandleGrabPoint").transform;
		handleInitialGrabPoint = transform.Find ("HandleInitialGrabPoint");
		handleMinPos = transform.Find ("HandleMinPos");
		handleMaxPos = transform.Find ("HandleMaxPos");
		handleShootStartPoint = transform.Find ("HandleShootStartPoint");
		handleShootEndPoint = transform.Find ("HandleShootEndPoint");
	}
	
	void Update () {
	
		// Get the current position of the mouse, relative to the coordinate system
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		
		// Adjust the position of the cannon
		drawCannon(mousePos);	
			
//		if(Input.GetMouseButtonDown(0)) {
//			// If user pressed close enough to the center of the cannon, remember it
//			if(Vector3.Distance(mousePos, handleGrabPoint.position) <= grabRadius) {
//				readyToShoot = true;
//			}
//		} else if (Input.GetMouseButtonUp(0) && readyToShoot) {
//			// If user released and had already pressed in a valid position, shoot
//			
//			// First, calculate the force of the shot
//			Vector3 force = (-1f * handle.position) * shootForce;
//			
//			// Then, instantiate a bullet prefab, and apply the force
//			Vector3 bulletPos = calculateBulletPos();
//			GameObject newBullet;
//			if(bulletPool.Count == 0) {
//				newBullet = Instantiate(bulletPrefab) as GameObject;
//			} else {
//				newBullet = bulletPool.Pop();
//			}
//			newBullet.GetComponent<BulletController>().ShootBullet(bulletPos, force);
//			
//			// Reset flag
//			readyToShoot = false;
//		}
		
		if(Input.GetMouseButtonDown(0)) {
			// If user pressed close enough to the handle grab point, remember it
			if(Vector3.Distance(mousePos, handleGrabPoint.position) <= grabRadius) {
				handleIsGrabbed = true;
			}
		}
		
		if(Input.GetMouseButtonUp(0)) {
			handleIsGrabbed = false;
			readyToShoot = false;
		}
		
		if(handleIsGrabbed) {
			if(handle.position.y < handleShootStartPoint.position.y) {
				lastTimeAtStartPoint = Time.timeSinceLevelLoad;
				readyToShoot = true;
			}
			if(handle.position.y > handleShootEndPoint.position.y) {
				lastTimeAtEndPoint = Time.timeSinceLevelLoad;
				if(readyToShoot) {
					//First, calculate the force of the shot
					float forceScalar = 1f/(lastTimeAtEndPoint - lastTimeAtStartPoint) * shootForce;
					Vector3 forceVector = -1f * handle.position.normalized * forceScalar;
					
					// Then, instantiate a bullet prefab, and apply the force
					Vector3 bulletPos = calculateBulletPos();
					GameObject newBullet;
					if(bulletPool.Count == 0) {
						newBullet = Instantiate(bulletPrefab) as GameObject;
					} else {
						newBullet = bulletPool.Pop();
					}
					newBullet.GetComponent<BulletController>().ShootBullet(bulletPos, forceVector);
					
					readyToShoot = false;
				}
			}
		}
		
		
	}
	
	// Manages the rotation of the cannon's sprite
	void drawCannon(Vector3 mousePos) {
//		if(!readyToShoot) return; // Do not change if not pressed
		if(!handleIsGrabbed) return; // Do not change if not pressed
		
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
		if (mousePos.sqrMagnitude >= handleInitialGrabPoint.position.sqrMagnitude) {
			float handleNewLocalYPos = -Vector3.Distance(mousePos, handleInitialGrabPoint.position);
			if(handleNewLocalYPos <= handleMinPos.localPosition.y
			&& handleNewLocalYPos >= handleMaxPos.localPosition.y) {
				handle.localPosition = new Vector3(0, handleNewLocalYPos, 0);
			}
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
