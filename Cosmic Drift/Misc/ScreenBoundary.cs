using UnityEngine;
using System.Collections;

public class ScreenBoundary : MonoBehaviour {
	public bool rightBoundary;

	void OnTriggerEnter2D(Collider2D col){
		if(rightBoundary){
			if (col.gameObject.CompareTag ("Bullet"))
				col.gameObject.SetActive (false);
		} else {
			if (col.gameObject.transform.parent != null)
				GameObject.Destroy (col.transform.parent.gameObject);
			else
				GameObject.Destroy (col.gameObject); 
		}
	}
}
