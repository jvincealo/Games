using UnityEngine;
using System.Collections;

public class PlayerSightChecker : MonoBehaviour {
	RaycastHit hit;
	float rayTimer;
	Player player;
	int layerMask;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
		layerMask = LayerMask.GetMask ("Default", "Jumpscares");
	}
	
	// Update is called once per frame	
	void Update () {
		rayTimer += Time.deltaTime;
		Debug.DrawRay (transform.position, transform.forward * 15, Color.cyan);
		if(rayTimer >= 0.1f){
			if(Physics.Raycast(transform.position, transform.forward, out hit, 15, layerMask)){
				Debug.Log (hit.collider.name);
				JumpScare jumpScare = hit.collider.gameObject.GetComponentInParent<JumpScare> ();
				if(jumpScare != null && player.isFlashlightOn()){
					jumpScare.startJumpScare ();
				}
			}
			rayTimer = 0;
		}
	}
}
