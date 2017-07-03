using UnityEngine;
using System.Collections;

public class ItemPickup : MonoBehaviour {
	public string itemName; //coins, power-up 1, power-up 2, etc
	public bool powerUp;

	GameObject playerObj;
	bool magnetized;

	void Start(){
		if(powerUp){
			GetComponent<Rigidbody2D>().AddForce (new Vector2 (-100, 0));
		}
	}

	void FixedUpdate(){
		if(magnetized){
			transform.position = Vector3.MoveTowards (transform.position, playerObj.transform.position, Time.deltaTime*10);
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.CompareTag("Player")){
			Player player = col.gameObject.GetComponent<Player> ();
			player.Pickup (itemName);
			GameObject.Destroy (gameObject);
		}
		if(itemName == "coin" && col.CompareTag("Magnet") && !magnetized){
			magnetized = true;
			playerObj = col.gameObject;
		}
	}
}
