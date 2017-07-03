using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class EnemyAI : MonoBehaviour {
	public NavMeshAgent agent;
	public Enemy enemy;

	Vector3 direction;

	public State state;
	bool isAlive;
	LayerMask mask;

	AudioSource enemyAudio;

	//IDLE
	float idleTimer = 0;

	//PATROL
	public GameObject[] waypoints;
	int waypointInd = 0;
	public float patrolSpeed;

	//CHASE
	public float chaseSpeed;
	public Player target; //PLAYER
	NavMeshPath pathTemp;

	//INVESTIGATE
	Vector3 investigateSpot;
	public float investigateWait = 7.5f;

	//ENEMY SIGHT
	public float heightMultiplier;
	public float sightAngle = 120;
	SphereCollider detectRadius; //SIGHT RANGE
	float originalRadius;
	RaycastHit hit;

	//ATTACK
	float range = 5;

	//CHASE TIMER (for panic audio)
	float chaseTimer = 0;
	bool countIncreased;
	bool countDecreased = true;

	public enum State {
		IDLE,
		PATROL,
		CHASE,
		INVESTIGATE,
		IN_RANGE
	}

	void Start () {
		pathTemp = new NavMeshPath();
		mask = LayerMask.GetMask ("Default", "Player");
		enemyAudio = GetComponent<AudioSource>();
		detectRadius = GetComponent<SphereCollider>();
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		agent = GetComponent<NavMeshAgent>();
		enemy = GetComponent<Enemy>();
		agent.updateRotation = false;
		agent.updatePosition = true;

		state = State.IDLE;
		isAlive = true;

		heightMultiplier = 1.85f;
		originalRadius = detectRadius.radius;

		StartCoroutine(EnemyStateMachine());
	}

	void Update(){
		if(state == State.IN_RANGE || state == State.CHASE) chaseTimer += Time.deltaTime; 
		else chaseTimer = 0;

		if(chaseTimer >= 1.5f && !countIncreased && isAlive){ //if chasing for at least 1.5s and enemy hasn't been counted
			target.updateEnemyChasingCount(true); //increment chasing count
			countIncreased = true;
			countDecreased = false;
		}
	
	}

	IEnumerator EnemyStateMachine(){
		while(isAlive){
			switch(state){
			case State.IN_RANGE:
				idleTimer = 0;
				InRange();
				break;
			case State.IDLE: //stay for a few seconds after reaching waypoints
				Idle();
				detectRadius.radius = originalRadius;
				break;
			case State.PATROL:
				idleTimer = 0;
				Patrol();
				break;
			case State.CHASE:
				idleTimer = 0;
				Chase ();
				detectRadius.radius = originalRadius * 2; //double the radius--increases distance before zombie stops chasing
				break;
			case State.INVESTIGATE:
				idleTimer = 0;
				Investigate();
				break;
			}
			yield return null;
		}
	}

	void Idle(){
		enemy.Move(Vector3.zero);
		agent.speed = 0; //prevents agent from moving since proc'ing Chase STATE gives speed and a destination even when back to IDLE
		if(waypoints.Length != 0){
			idleTimer += Time.deltaTime;
			if (idleTimer >= Random.Range(2.5f, 5))
				state = State.PATROL;
		}
	}

	void Patrol(){
		agent.speed = patrolSpeed;
		if(Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) >= 2){
			agent.SetDestination(waypoints[waypointInd].transform.position);
			enemy.Move(agent.desiredVelocity);
		} else if(Vector3.Distance(this.transform.position, waypoints[waypointInd].transform.position) <= 2){
			waypointInd += 1; //go to next waypoint
			if(waypointInd >= waypoints.Length) waypointInd = 0; //reset waypoint
			state = State.IDLE;
		} else {
			enemy.Move(Vector3.zero);
		}
	}

	void InRange(){
		Vector3 targetDir = target.transform.position;
		targetDir.y -= 2.05f;
		if(target.isAlive()){
			if (Vector3.Distance(transform.position, targetDir) > range) { //not in range
				agent.Resume();
				state = State.CHASE;
				enemy.Attack(false);
			} else { //in range
				agent.Stop();
				enemy.Attack(true);
			}
		} else {
			enemy.Attack(false);
			state = State.IDLE;
		}
	}

	void Chase(){
		Vector3 targetDir = target.transform.position;
		targetDir.y -= 2.05f;
		if(Vector3.Distance(transform.position, targetDir) <= range) { //melee range, attack player
			state = State.IN_RANGE;
		} else {
			agent.speed = chaseSpeed;
			agent.ResetPath();
			agent.SetDestination(targetDir);
			enemy.Move(agent.desiredVelocity);
			if (!enemy.anim.GetCurrentAnimatorStateInfo (0).IsName ("run"))
				agent.Stop ();
			else
				agent.Resume ();
		}
	}
		
	void Investigate(){
		Vector3 targetDir = target.transform.position;
		targetDir.y -= 2.05f;
		if (Vector3.Distance (targetDir, transform.position) <= 3.5f) { //for player collision, rotate 180
			transform.Rotate (transform.up, 180);
			enemy.Move(Vector3.zero);
		} else if(Vector3.Distance(transform.position, investigateSpot) >= 2){ //go to sound source
			agent.speed = patrolSpeed;
			agent.ResetPath();
			agent.SetDestination(investigateSpot);
			enemy.Move(agent.desiredVelocity);
		} else { //reached sound source
			state = State.IDLE;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.tag == "Player"){
			if (state == State.CHASE) {
				enemyAudio.Stop();
				state = State.IDLE; //too far, stop chasing and stay idle for few seconds
				if(!countDecreased){ //if enemy is not chasing anymore and hasnt been decremented in count
					target.updateEnemyChasingCount(false); 
					countDecreased = true;
					countIncreased = false;
				} 
			}
		}
	}

	void OnTriggerStay(Collider col){
		if(col.tag == "Player" && target.isAlive() && isAlive){
			Vector3 targetDir = col.transform.position;
			targetDir.y -= 2.05f; //disregard y value of points to compute in 2d angle
			if(Vector3.Distance(targetDir, transform.position) <= 3.5f && state != State.CHASE && state != State.IN_RANGE && state != State.INVESTIGATE){ //player is VERY near 
				investigateSpot = targetDir;
				state = State.INVESTIGATE;
			}
			if(state != State.IN_RANGE){
				RaycastHit hit;
				RaycastHit hit2;

				Vector3 currentPosition = transform.position;
				direction = targetDir - currentPosition;
				float angle = Vector3.Angle(direction, transform.forward);
				// within enemy's field of view; SIGHT-BASED DETECTION
				if(Vector3.Dot(direction, transform.forward) >= 0 && angle <= sightAngle/2){ 
//					Debug.DrawRay(transform.position + transform.up*2 , direction.normalized*detectRadius.radius, Color.blue);
					if(Physics.Raycast(transform.position + transform.up*2, direction.normalized, out hit, detectRadius.radius, mask)){
						Debug.Log (hit.collider.gameObject.name);
						if(hit.collider.gameObject == target.gameObject){
							state = State.CHASE; //if enemy can see player and within detection range, chase 
						} else {
							CheckBlockedPath (targetDir);
						}
					}
				//not within sight range but already chasing => continue chase
				} else if(state == State.CHASE && CalculatePathLength(col.transform.position) <= detectRadius.radius){
					CheckBlockedPath (targetDir);
				} else { // not within enemy's field of view; SOUND-BASED DETECTION
					//check first if player silently entered detection radius
					if(target.IsMakingSounds()){ //player is not silent; dont investigate other places until current spot is finished
						if(CalculatePathLength(col.transform.position) <= (2*detectRadius.radius)/3){ //if player is loud while within sight radius
							agent.CalculatePath (targetDir, pathTemp);
							if(pathTemp.status == NavMeshPathStatus.PathPartial){ //if there's a path but blocked (closed door while chasing)
								state = State.IDLE;
								if(!countDecreased){
									target.updateEnemyChasingCount(false); 
									countIncreased = false;
									countDecreased = true;
								}
							} else { //if there's a valid path
								Debug.Log("PATH LENGTH IS: "+CalculatePathLength(col.transform.position));
								state = State.CHASE;
							}
						} else if(CalculatePathLength(col.transform.position) <= detectRadius.radius && state != State.INVESTIGATE){ //calculate if player is within range (same room)
							investigateSpot = targetDir;
							state = State.INVESTIGATE;
						}
					}
				}

				//Player RAYCAST for SANITY
				Vector3 direction2 = currentPosition - targetDir;
				float angle2 = Vector3.Angle (direction2, col.transform.forward);
				if (Physics.Raycast (col.transform.position + col.transform.up * 2, direction2.normalized, out hit2, detectRadius.radius)) {
					//if raycast hit the enemy; player has vision of enemy--reduce sanity
					if(hit2.collider.gameObject.layer == 8 || hit2.collider.gameObject.layer == 10){ 
						if (Vector3.Dot (direction2, col.transform.forward) >= 0) {
							if (angle2 <= 55) {
								target.updateSanity (true, 0.000625f);
							}
						}
					}
				}
			}
		}
		if (col.tag == "Bait" && state != State.INVESTIGATE && state != State.CHASE) { //if the bait thrown is within hearing radius; dont investigate other places until current spot is finished
			Bait temp = col.gameObject.GetComponent<Bait> ();
			if (temp.isMakingSounds ()) { //once the bait has made a sound
				if(CalculatePathLength(temp.gameObject.transform.position) <= detectRadius.radius){ //calculate if player is within range (same room)
					investigateSpot = temp.gameObject.transform.position;
					state = State.INVESTIGATE;
				}
			}
		}
	}

	void CheckBlockedPath(Vector3 targetDir){ //update chase if no path is available for chasing
		agent.CalculatePath (targetDir, pathTemp);
		if (pathTemp.status == NavMeshPathStatus.PathPartial) { //if there's a path but blocked (closed door while chasing)
			state = State.IDLE;
			if (!countDecreased) {
				target.updateEnemyChasingCount (false); 
				countIncreased = false;
				countDecreased = true;
			}
		} else
			state = State.CHASE;
	}

	float CalculatePathLength(Vector3 targetPosition){
		NavMeshPath path = new NavMeshPath();
		Vector3 trueTargetPosition = new Vector3 (targetPosition.x, targetPosition.y - 2.05f, targetPosition.z); //player y pos has +2 offset
		float pathLength = 0;

		if(agent.enabled){
			agent.CalculatePath(trueTargetPosition, path); //store the calculated path in 'path'

			Vector3[] allWaypoints = new Vector3[path.corners.Length + 2]; //store all corners + the player and enemy's position here

			allWaypoints[0] = transform.position; //store enemy position
			allWaypoints[allWaypoints.Length - 1] = trueTargetPosition; //store player position

			for(int i=0; i<path.corners.Length; i++){ //store all of the corner's position
				allWaypoints[i + 1] = path.corners[i];
			}

			for(int i=0; i<allWaypoints.Length-1; i++){
				pathLength += Vector3.Distance(allWaypoints[i], allWaypoints[i + 1]); //calculate the total path length
			}
				
		}
		return pathLength;
	}

	void FixedUpdate(){
		Debug.DrawRay(transform.position + Vector3.up*2 , (Quaternion.AngleAxis(sightAngle/2f, transform.up) * transform.forward).normalized * detectRadius.radius, Color.green);
		Debug.DrawRay(transform.position + Vector3.up*2 , (Quaternion.AngleAxis(-1*sightAngle/2f, transform.up) * transform.forward).normalized * detectRadius.radius, Color.green);
	}

	public void detectShooter(){ //chase player if enemy is shot/hit by bait and within range
		Vector3 targetDir = target.transform.position;
		targetDir.y -= 2.05f;
		if (Vector3.Distance(transform.position, targetDir) < originalRadius * 2 && state != State.CHASE){
			if (Vector3.Distance(transform.position, targetDir) <= originalRadius) detectRadius.radius = originalRadius * 2;
			investigateSpot = targetDir;
			state = State.INVESTIGATE;
		}
	}

	public void Die(){
		if(isAlive){
			agent.ResetPath();
			isAlive = false;
			detectRadius.enabled = false;
//			gameObject.GetComponentInChildren<BoxCollider> ().gameObject.tag
			GameObject.Destroy(gameObject.GetComponentInChildren<StealthKillScript> ().gameObject);
			GameObject.Destroy(gameObject.GetComponentInChildren<Rigidbody> ().gameObject);
			if (!countDecreased){
				target.updateEnemyChasingCount (false);
				countDecreased = true;
			}
		}
	}

}