using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	PlayerController player;
	GameManager gameAudio;

	int debrisLayer;
	int enemyLayer;
	int incomingObsLayer; //incoming obstacle layer mask for raycasting powerups
	public GameUI gameUI;
	public GameObject magnet;
	public CircleCollider2D boosterCollider;
	public float jetpackForce;
	public float forwardSpeed;
	public float increaseRate; //rate of speed increase per minute
	public int bulletAmmo;
	public int maxAmmo;

	bool alive = true;
	bool boostedSpeed;
	bool magnetized;
	bool dashing;
	int bulletLevel = 0;
	float rechargeTimer; //bullet recharge timer
	int timeToRecharge = 3;
	int currentSkill = 0; //0 - dash, 1 - wipe
	int skillAmmo = 0;
	float skillCooldown; //cooldown timer
	float cooldownTime = 6; //skillCooldown
	bool onCooldown;
	float boosterTimer;
	float magnetTimer;
	float dashTimer;

	int coins;
	int highscoreCoins; //highscore
	float distanceTraveled;
	float farthestDistance; //highscore

	void Start(){
		player = GetComponent<PlayerController> ();
		gameAudio = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<GameManager>();
		enemyLayer = LayerMask.NameToLayer ("Enemy");
		debrisLayer = LayerMask.NameToLayer ("Debris");
		incomingObsLayer = LayerMask.GetMask ("Debris", "Enemy");
		highscoreCoins = PlayerPrefs.GetInt ("coins", highscoreCoins);
		farthestDistance = PlayerPrefs.GetFloat ("distance", farthestDistance);
	}

	void Update(){
		RechargeAmmo ();
		UpdateSkillCooldown ();
	}

	void RechargeAmmo(){
		if(bulletAmmo < maxAmmo){ //less than max bullets, start recharging ammo
			rechargeTimer += Time.deltaTime;
			gameUI.UpdateCooldown (rechargeTimer/timeToRecharge, true);
			if(rechargeTimer >= timeToRecharge){ //5s recharge time
				rechargeTimer = 0;
				bulletAmmo++;
				gameUI.UpdateAmmo (bulletAmmo, true);
			}
		}
	}

	void FixedUpdate(){
		CheckIncomingObstacle ();
		IncreaseSpeed ();

		UpdatePowerUpState ();
		UpdateDashState ();

	}

	bool CheckIncomingObstacle(){ //nearby obstacle checker to ensure safe termination of super fast speeds---booster/dash
		Vector2 midRayPos = new Vector2 (transform.position.x-0.75f, transform.position.y - 0.05f);
		Vector2 midTopRayPos = new Vector2 (transform.position.x-0.75f, transform.position.y + 0.5f);
		Vector2 midBotRayPos = new Vector2 (transform.position.x-0.75f, transform.position.y - 0.55f);
		Vector2 topRayPos = new Vector2 (transform.position.x-0.75f, transform.position.y + 1);
		Vector2 bottomRayPos = new Vector2 (transform.position.x-0.75f, transform.position.y - 1.05f);
		RaycastHit2D hit = Physics2D.Raycast(midRayPos, Vector2.right, 10, incomingObsLayer);
		RaycastHit2D hit2 = Physics2D.Raycast (topRayPos, Vector2.right, 10, incomingObsLayer);
		RaycastHit2D hit3 = Physics2D.Raycast(bottomRayPos, Vector2.right, 10, incomingObsLayer);
		RaycastHit2D hit4 = Physics2D.Raycast(midTopRayPos, Vector2.right, 10, incomingObsLayer);
		RaycastHit2D hit5 = Physics2D.Raycast(midBotRayPos, Vector2.right, 10, incomingObsLayer);
		if (hit.collider != null || hit2.collider != null || hit3.collider != null || hit4.collider != null || hit5.collider != null) { //there's an incoming obstacle
			return true;
		} else {
			return false;
		}
	}

	void IncreaseSpeed(){ //increase speed constantly
		if(alive){
			distanceTraveled += Time.deltaTime * forwardSpeed / 1000;
			gameUI.UpdateDistance (distanceTraveled);
			forwardSpeed += Time.deltaTime * increaseRate/60; //+1 speed per minute
		}
	}

	void UpdatePowerUpState(){ //manage powerup duration
		if(boostedSpeed){
			boosterTimer += Time.deltaTime;
			if(boosterTimer >= 5 && !CheckIncomingObstacle()){ //stop if booster duration is up and no incoming obstacles are near
				boostedSpeed = false;
				forwardSpeed /= 10;
				boosterTimer = 0;
				Physics2D.IgnoreLayerCollision (gameObject.layer, enemyLayer, false);
				Physics2D.IgnoreLayerCollision (gameObject.layer, debrisLayer, false);
				boosterCollider.enabled = false;
			}
		}
		if(magnetized){
			magnetTimer += Time.deltaTime;
			if(magnetTimer >= 10){
				magnetized = false;
				magnet.SetActive (false);
				magnetTimer = 0;
			}
		}
	}

	void UpdateDashState(){ //manage duration of dash 
		if(dashing){
			dashTimer += Time.deltaTime;
			if(dashTimer >= 0.2f && !CheckIncomingObstacle()){ //stop if dash duration is up and no incoming obstacles are near
				dashing = false;
				forwardSpeed /= 10;
				dashTimer = 0;
				Physics2D.IgnoreLayerCollision (gameObject.layer, enemyLayer, false);
				Physics2D.IgnoreLayerCollision (gameObject.layer, debrisLayer, false);
			}
		}
	}

	void UpdateSkillCooldown(){ //manage skill cooldown
		if(onCooldown){
			skillCooldown += Time.deltaTime;
			gameUI.UpdateCooldown (skillCooldown / cooldownTime, false);
			if(skillCooldown >= cooldownTime){
				onCooldown = false;
				skillCooldown = 0;
			}
		}
	}

	public void Pickup(string name){
		switch(name){
			case "coin":
				IncrementCoins ();
				break;
			case "booster":
				BoosterPowerUp ();
				break;
			case "magnet":
				MagnetPowerUp ();
				break;
			default:
				UpdateCurrentSkill (name);
				break;
		}
	}

	void IncrementCoins(){
		gameAudio.PlaySfx ("coin");
		coins++;
		gameUI.UpdateCoins (coins);
	}

	void BoosterPowerUp(){
		gameAudio.PlaySfx ("booster");
		if(dashing){
			dashing = false;
			dashTimer = 0;
			forwardSpeed /= 10;
		}
		Physics2D.IgnoreLayerCollision (gameObject.layer, enemyLayer);
		Physics2D.IgnoreLayerCollision (gameObject.layer, debrisLayer);
		forwardSpeed *= 10;
		boostedSpeed = true;
		boosterCollider.enabled = true;
	}

	void MagnetPowerUp(){
		gameAudio.PlaySfx ("magnet");
		magnet.SetActive (true);
		magnetized = true;
	}

	void UpdateCurrentSkill(string skill){
		switch(skill){
			case "dash":
				UpdateSkillEffect (0, 5, 6);
					break;
			case "wipeout":
				UpdateSkillEffect (1, 2, 10);
				break;
		}
	}

	void UpdateSkillEffect(int skillNumber, int ammo, int cooldown){
		gameUI.UpdateSkill (skillNumber);
		if(currentSkill == skillNumber){
			skillAmmo += ammo;
		} else {
			currentSkill = skillNumber;
			skillAmmo = ammo;
			onCooldown = false;
			skillCooldown = 0;
			gameUI.UpdateCooldown (1, false);
		}
		gameUI.UpdateAmmo (skillAmmo, false);
		cooldownTime = cooldown;
	}

	public void UseSkill(){
		if(skillAmmo > 0 && !onCooldown && !boostedSpeed){ //disable skill use if on cooldown and on booster and have ammo
			switch(currentSkill){
			case 0:
				Dash ();
				break;
			case 1:
				Wipe ();
				break;
			}
			onCooldown = true;
			skillAmmo--;
			gameUI.UpdateAmmo (skillAmmo, false);
		}
	}

	void Dash(){
		gameAudio.PlaySfx ("dash");
		forwardSpeed *= 10;
		dashing = true;
		Physics2D.IgnoreLayerCollision (gameObject.layer, enemyLayer);
		Physics2D.IgnoreLayerCollision (gameObject.layer, debrisLayer);
	}

	void Wipe(){
		gameAudio.PlaySfx ("wipeout");
		player.ShootWipeout ();
	}
		

	public bool IsBoosterOn(){
		return boostedSpeed;
	}

	public bool IsDashOn(){
		return dashing;
	}

	public float GetJetpackForce(){
		return jetpackForce;
	}

	public float GetForwardSpeed(){
		return forwardSpeed;
	}

	public void UpgradeBullet(){
		if(coins >= (bulletLevel + 1) * 20){ //sufficient coins, upgrade
			coins -= (bulletLevel + 1) * 20;
			bulletLevel++;
			gameUI.UpdateCoins (coins);
		}
	}

	public int GetBulletLevel(){
		return bulletLevel;
	}

	public bool HasAmmo(){
		return bulletAmmo > 0;
	}

	public void DecrementAmmo(){
		bulletAmmo--;
		gameUI.UpdateAmmo (bulletAmmo, true);
		gameAudio.PlaySfx ("gun");
	}

	public bool isAlive(){
		return alive;
	}

	public void Die(){
		if(alive){
			gameAudio.PlaySfx ("player");
			GetComponent<Animator> ().SetTrigger ("death");
			alive = false;
			Rigidbody2D rBody = GetComponent<Rigidbody2D> ();
			rBody.velocity = Vector3.zero;
			rBody.gravityScale = 0;
			GetComponent<PlayerController> ().enabled = false;
			GetComponent<CircleCollider2D> ().enabled = false;
			GetComponentInChildren<ParticleSystem> ().gameObject.SetActive (false);
		}
	}

	public void EndGame(){ // Record highscores if needed and show end screen
		gameUI.ShowEndScreen (distanceTraveled, coins, farthestDistance, highscoreCoins);
		if(distanceTraveled > farthestDistance){ //HIGH SCORE is determined by distance, regardless of coins
			farthestDistance = distanceTraveled;
			highscoreCoins = coins;
			PlayerPrefs.SetFloat ("distance", farthestDistance); //save farthrest distance with corresponding coins
			PlayerPrefs.SetInt ("coins", highscoreCoins);
			PlayerPrefs.Save ();
		}
	}
}
