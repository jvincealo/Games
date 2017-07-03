using UnityEngine;
using System.Collections;

public class Wipeout : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col){
		if (!(col.CompareTag ("ScreenBoundary"))) {
			col.gameObject.GetComponent<Animator> ().SetTrigger ("explode");
			col.gameObject.GetComponent<Collider2D> ().enabled = false;
		}
	}
}
