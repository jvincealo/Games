using UnityEngine;
using System.Collections;

public class Bait : MonoBehaviour {
	public GameObject baitPickupPrefab;
	public AudioClip baitSound;
	public AudioClip baitHitSound;

	bool isLoud;
	bool hasCollided;
	bool hasHit;
	bool hasForce;
	Rigidbody rBody;
	AudioSource baitAudio;


	void Awake(){
		rBody = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {

		baitAudio = GetComponent<AudioSource> ();
		GameObject temp = GameObject.FindGameObjectWithTag ("Player");
		Physics.IgnoreCollision (temp.GetComponent<CharacterController>(), GetComponent<MeshCollider>(), true);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		if(rBody.velocity != Vector3.zero){ //need to check first if object gained force (the first rBody.velocity==0 executes before ProjectileThrower adds force)
			hasForce = true; 
		} else if(rBody.velocity == Vector3.zero && hasForce){
			GameObject baitPickup = Instantiate (baitPickupPrefab, transform.position, transform.localRotation) as GameObject;
			GameObject.Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision collision){
		if(collision.collider.gameObject.layer == 8 && !hasHit ){ //layer 8 = SHOOTABLE
			EnemyAI enemy = collision.collider.gameObject.GetComponentInParent<EnemyAI> ();
			if (enemy != null) { //for the sake of catching exception; layer 8 only contains enemy
				enemy.detectShooter(); //investigate the spot the player threw the bait on
			}
			baitAudio.PlayOneShot (baitHitSound);
			hasHit = true;
		} else if(collision.collider.gameObject.layer == 0 && !hasCollided){
			Debug.Log (collision.collider.gameObject.name);
			baitAudio.PlayOneShot (baitSound);
			hasCollided = true;
			isLoud = true;
			hasHit = true; //enemy detection not called when collision happens during the bouncing motion
			Debug.Log ("GROUND");
		}
	}

	public bool isMakingSounds(){
		return isLoud;
	}
}
