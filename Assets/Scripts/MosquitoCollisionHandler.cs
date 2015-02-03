using UnityEngine;
using System.Collections;

public class MosquitoCollisionHandler : MonoBehaviour {
	
	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("bullet")) {
			transform.GetComponentInParent<EnemyController>().Dissapear();
		}
	}
}
