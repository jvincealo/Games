using UnityEngine;
using System.Collections;

public class FixRotation : MonoBehaviour { //for old cup pickup, thrown cup instances have different initial transform.rotation
	Vector3 temp;

	void Start(){
		Vector3 temp = transform.localRotation.eulerAngles;
		transform.GetChild(0).localRotation = Quaternion.Euler(360 - temp.x, 360 - temp.x, 270 - temp.z);
	}

}	
