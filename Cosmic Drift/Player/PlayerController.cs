using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {
	public ObjectPooler bulletPool;

	Player player;
	Animator playerAnim;
	GameObject bulletSpawnPoint;
	GameObject feetPosition;
	Rigidbody2D rBody;
	ParticleSystem jetpackFlames;
	float shotIntervalTimer;
	int groundLayer;
	bool grounded;
	bool isButtonHeld;

	void Start () {
		rBody = GetComponent<Rigidbody2D> ();
		jetpackFlames = GetComponentInChildren<ParticleSystem> ();
		player = GetComponent<Player> ();
		playerAnim = GetComponent<Animator> ();
		bulletSpawnPoint = transform.GetChild (1).gameObject;
		feetPosition = transform.GetChild (2).gameObject;
		groundLayer = LayerMask.GetMask ("Ground");
	}

	void Update () { //user-controlled inputs & non-physics related functions
		ControlBlaster (); //shoot bullet prefabs
		ControlJetpack (); //flying movement
	}

	void FixedUpdate(){ //physics-related input
		MovePlayer(); //forward movement

		CheckGrounded(); //walk animation enable/disable

	}

	void ControlJetpack(){
		if(Input.touchCount > 0 && Input.GetTouch (0).phase != TouchPhase.Ended && Input.GetTouch (0).phase != TouchPhase.Canceled){ //activate jetpack; fly
			if (!(EventSystem.current.IsPointerOverGameObject (Input.GetTouch (0).fingerId))) { //dont activate jetpack on UI button touch
//		if(Input.GetKey(KeyCode.Space)){ //pc testing
			if(!player.IsBoosterOn()) 
				jetpackFlames.Play ();
			rBody.AddForce (new Vector2 (0, player.GetJetpackForce() * rBody.mass));
			}
		} else { //don't use jetpack
			jetpackFlames.Stop ();
		}
	}

	public void ToggleBlaster(bool value){ //called on button up/down
		isButtonHeld = value;
	}

	void ControlBlaster(){
		shotIntervalTimer += Time.deltaTime;
		if(shotIntervalTimer >= 0.2f && isButtonHeld && player.HasAmmo()){
			GameObject bullet = bulletPool.GetBullet (player.GetBulletLevel ()); //get object from object pool
			bullet.transform.position = bulletSpawnPoint.transform.position; //place bullet in bullet's spawn point
			bullet.SetActive (true);
			Rigidbody2D temp = bullet.GetComponent<Rigidbody2D> ();
			temp.AddForce (new Vector2 (300 * player.GetForwardSpeed() * temp.mass, 0)); //fire bullet
			shotIntervalTimer = 0;
			player.DecrementAmmo ();

		}
	}

	public void ShootWipeout(){
		GameObject wipeout = bulletPool.GetWipeout ();
		wipeout.transform.position = new Vector2 (bulletSpawnPoint.transform.position.x + 0.5f, 0);
		Rigidbody2D temp = wipeout.GetComponent<Rigidbody2D> ();
		temp.AddForce (new Vector2 (150 * player.GetForwardSpeed () * temp.mass, 0));
	}

	void MovePlayer(){
		rBody.velocity = new Vector2(player.GetForwardSpeed(), rBody.velocity.y);

	}

	void CheckGrounded(){
		//Manage player animations here
		if(player.IsBoosterOn()){
			playerAnim.SetBool ("booster", true);
		} else if(player.IsDashOn()){
			playerAnim.SetBool ("dash", true);
		} else {
			Debug.DrawRay (feetPosition.transform.position, Vector3.down * 0.01f, Color.blue);
			RaycastHit2D hit = Physics2D.Raycast (feetPosition.transform.position, Vector2.down, 0.01f, groundLayer);
			playerAnim.SetBool ("grounded", (hit.collider != null)); //plays walk animation if raycast hits ground
			playerAnim.SetBool ("booster", false);
			playerAnim.SetBool ("dash", false);
		}
	}
		
	void OnCollisionEnter2D(Collision2D col){
		if(player.isAlive()){
			if(col.gameObject.CompareTag("Enemy")){
				Enemy enemy = col.gameObject.GetComponent<Enemy> ();
				enemy.TakeDamage (enemy.health);
				player.Die ();
			} else if(col.gameObject.CompareTag("Debris")){
				player.Die ();
				if (col.gameObject.GetComponent<Debris> ().destroyOnContact)
					GameObject.Destroy (col.gameObject);
			}
		}
	}
}
