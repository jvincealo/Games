using UnityEngine;
using System.Collections;

public class StealthKillScript : MonoBehaviour {
	public GameUI HUDManager;

	bool withinPlayerRange;
	bool startTimer;
	float animationTimer;

	Player player;
	Animator playerAnim;
	Enemy enemy;
	EnemyAI enemyState;
	GameObject stealthUI;

	void Start (){
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		playerAnim = GameObject.FindGameObjectWithTag ("Arms").GetComponent<Animator> ();
		enemy = GetComponentInParent<Enemy>();
		enemyState = GetComponentInParent<EnemyAI>();
		stealthUI = transform.GetChild (0).gameObject;
	}

	void Update () {
		if(player.hasWeapons()){
			if (Input.GetKeyDown (KeyCode.E) && withinPlayerRange) {
				playerAnim.SetTrigger ("stealthKill");
				startTimer = true;
			}

			if (startTimer) {
				animationTimer += Time.deltaTime;
			}

			if (animationTimer >= .5) {
				ToggleStealthUI (false);
				HUDManager.StealthFlashImage ();
				enemy.TakeDamage (100);
				enemy.Die (false);
				GameObject.Destroy (gameObject);
			}
		}
	}

	void OnTriggerStay(Collider other){
		if(other.tag == "Player" && player.hasWeapons() && (enemyState.state != EnemyAI.State.CHASE && enemyState.state != EnemyAI.State.IN_RANGE)){
			Vector3 playerPos = other.gameObject.transform.position;
			Vector3 enemyPos = gameObject.transform.position;
			playerPos.y = 0; 
			enemyPos.y = 0;
			Vector3 direction = playerPos - enemyPos;
			Vector3 direction2 = enemyPos - playerPos;
			//if player is behind enemy
			if(Vector3.Dot(direction, gameObject.transform.forward) < 0 && enemyState.state != EnemyAI.State.CHASE && Vector3.Angle(direction2, other.gameObject.transform.forward) < 15){ 
				withinPlayerRange = true;
				ToggleStealthUI(true);
			} else {
				withinPlayerRange = false;
				ToggleStealthUI(false);
			}
		}	
	}

	void OnTriggerExit(Collider other){
		withinPlayerRange = false;
		ToggleStealthUI(false);
	}

	void ToggleStealthUI(bool value){
		stealthUI.SetActive (value);
	}
}
