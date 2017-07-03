using UnityEngine;
using System.Collections;

public class JumpScare : MonoBehaviour {
	GameObject player;
	public GameManager gameManager;
	public int jumpScareType;
	public float moveSpeed;
	Vector3 targetSpot;
	bool startScare;

	NavMeshAgent agent;
	Animator anim;
	GameObject meshRenderers;
	GameObject hipColliders;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		agent = GetComponent<NavMeshAgent> ();
		anim = GetComponent<Animator> ();
		meshRenderers = GetComponentInChildren<SkinnedMeshRenderer> ().transform.parent.gameObject;
		hipColliders = transform.GetChild (0).gameObject;
		meshRenderers.SetActive (false);
		hipColliders.SetActive (false);
	}

	void Update () {
		if(startScare){
			switch(jumpScareType){
			case 1: //walk towards player
				MoveForward();
				break;
			}
		}
	}

	void MoveForward(){
		if(Vector3.Distance(transform.position, targetSpot) >= 0.5f){
			agent.speed = moveSpeed;
			agent.SetDestination (targetSpot);
		} else {
			agent.Stop ();
			anim.SetTrigger ("endJumpscare");
			GetComponent<BoxCollider> ().enabled = false;
		}

	}

	public void startJumpScare(){
		if(!startScare){
			Vector3 targetDir = player.transform.position;
			Vector3 currentPos = transform.position;
			targetDir.y = 0;
			currentPos.y = 0;
			Vector3 direction = targetDir - currentPos;
			if (Vector3.Dot (direction, transform.forward) < 0)
				transform.Rotate (transform.up, 180);
			gameManager.StartJumpscare ();
			meshRenderers.SetActive (true);
			hipColliders.SetActive (true);
			startScare = true;
			targetSpot = transform.position + transform.forward*2f;
			anim.SetTrigger("jumpscare1");
		}
	}
}
