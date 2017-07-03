using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PickupScript : MonoBehaviour {
	GameObject interactText;
	public GameUI HUDManager;
	public string itemName;

	bool withinPlayerRange;
	Player player;

	void Start (){
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		HUDManager = player.HUDManager;
		interactText = transform.GetChild (transform.childCount-1).gameObject;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.E) && withinPlayerRange) {
			ToggleLabel(false);
			player.PickupItem(itemName);
			GameObject.Destroy(gameObject);
		}
	}

	void OnTriggerStay(Collider other){
		if(other.tag == "Player"){
			//calculate first if player is facing the object
			Vector3 currentPos = other.gameObject.transform.position;
			Vector3 targetDir = transform.position;
			currentPos.y = 0;
			targetDir.y = 0;
			Vector3 direction = targetDir - currentPos;
			if ((Vector3.Dot(direction, other.gameObject.transform.forward) >= 0) && Vector3.Angle(direction, other.gameObject.transform.forward) < 60) {
				if((itemName == "Bait" && player.hasWeapons() && !player.hasCup()) || itemName != "Bait"){ //enable bait pickup once player has obtained the gun
					withinPlayerRange = true;
					ToggleLabel(true);
				}
			} else {
				if(withinPlayerRange){ //to make sure this code executes only once
					withinPlayerRange = false;
				 	ToggleLabel(false);
				}
			}
		}
	}

	void OnTriggerExit(Collider other){
		withinPlayerRange = false;
		ToggleLabel(false);
	}

	void ToggleLabel(bool value){
		interactText.SetActive (value);
	}

}
