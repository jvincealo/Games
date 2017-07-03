using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractUI : MonoBehaviour {
	Transform camTemp;
	// Use this for initialization
	void Start () {
		camTemp = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (camTemp);
		transform.Rotate (new Vector3 (0, 180, 0));
	}

}
