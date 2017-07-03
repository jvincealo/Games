using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float enemyHealth = 100;
	public float damage = 30;
	public AudioClip zombieSound;
	public AudioClip zombieScream;

	public Vector3 destination;
	public Vector3 direction;
	public float angle;
	public bool turning;

	bool isAlive = true;
	bool inMeleeRange;
	public float soundTimer = 0;

	public Animator anim;
	public ParticleSystem bloodSplat;
	EnemyAI currentState;
	Vector3 targetDir;
	AudioSource zombieAudio;
	public Vector3 currentPosition;

	void Awake () {
		anim = GetComponent<Animator>();
		bloodSplat = GetComponentInChildren<ParticleSystem>();
		currentState = GetComponent<EnemyAI>();
	}

	void Start(){
		zombieAudio = GetComponent<AudioSource>();
		turning = false;
		currentPosition = transform.position;
	}

	void Update(){
		soundTimer += Time.deltaTime;
		if(soundTimer >= 10 && !zombieAudio.isPlaying && currentState.state != EnemyAI.State.CHASE && currentState.state != EnemyAI.State.IN_RANGE && isAlive && currentState.target.isAlive()){
			zombieAudio.PlayOneShot(zombieSound);
			soundTimer = 0;
		}

		if(inMeleeRange){ //ensures that the zombie is facing the player during attack animation
			targetDir = currentState.target.transform.position;
			targetDir.y = 0;
			direction = targetDir - transform.position;
			Quaternion rotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*100);
		}
	}

	public void Move(Vector3 move){
		destination = move;
		if (currentState.state == EnemyAI.State.IDLE) { //IDLE
			anim.SetBool("isWalking", false);
			anim.SetBool("isRunning", false);
		} else { //IN MOTION
			if (currentState.state == EnemyAI.State.PATROL || currentState.state == EnemyAI.State.INVESTIGATE) {
				anim.SetBool("isWalking", true);
			} else {
				anim.SetBool("scream", true);
				anim.SetBool("isRunning", true);
			}

			//calculate angle and turn correspondingly
			targetDir = currentState.target.transform.position;

			//disregard y value of points to compute in 2d angle
			targetDir.y = 0; 
			currentPosition.y = 0;
			direction = targetDir - currentPosition;

			Quaternion rotation;
			if(currentState.state != EnemyAI.State.CHASE || move != Vector3.zero){
				rotation = Quaternion.LookRotation(move);
			} else{
				rotation = Quaternion.LookRotation(direction);
			}
			if(move != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime*100);
		}
	}

	public void Attack(bool inRange){
		if(inRange){
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("walk")){ //if in_range transitioned from investigate(walk) scream before attack
				anim.SetBool("scream", true);
				anim.SetBool("isRunning", true);
				inMeleeRange = false;
			} else {
				anim.SetBool("inRange", true);
				inMeleeRange = true;
			}
		} else {
			inMeleeRange = false;
			anim.SetBool("inRange", false);
		}
	}

	public void TakeDamage (float amount)
	{
		if(isAlive){
			currentState.detectShooter();
			if (amount >= 100) {
				bloodSplat.maxParticles = 100;
				ParticleSystem.ShapeModule temp = bloodSplat.shape;
				temp.radius = 0.5f;
			}
			bloodSplat.Stop();
			bloodSplat.Play();
			enemyHealth -= amount;
		}
	}

	public void Die(bool fallForward){
		if(isAlive){
			if(fallForward){
				anim.SetTrigger("death_forward");
			} else {
				anim.SetTrigger("death_backward");
			}

			isAlive = false;
			GetComponent<EnemyAI>().Die();
		}
	}

}
