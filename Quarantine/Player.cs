using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {
	public GameManager gameManager;
	public GameUI HUDManager;
	public float playerHealth = 100;
	public AudioClip hurtSound;
	public AudioClip flashlightOn;
	public AudioClip flashlightOff;
	public float timeBetweenShots = 0.4f;

	bool alive = true;
	bool isLoud;
	bool isDamaged;
	bool isHealed; //after finishing health kit
	bool isAiming;
	bool regenSanity;
	public bool[] hasKey;
	float damageTaken;
	float healingReceived;
	float shotTimer;
	float sanityTimer;
	float regenTimer;
	float healthKitTimer;
	float prevBlurAmount;
	public int enemyChasingCount = 0;

	//Player State
	int healthKitCount = 0;
	bool hasFlashlight;
	bool hasGun;
	public bool hasBait;
	bool readyThrow;
	bool throwWindUp;
	bool cancelThrow;
	bool gunActive;
	bool isHealing; //while holding healing key
	bool isReloading;

	AudioSource audioSource;
	GameObject mainCam;
	GunController gun;
	Animator armsAnim;
	Animator cameraAnim;
	Animator ammoTextAnim;
	Light[] flashlight;
	MotionBlur cameraSanity;
	ProjectileThrower trajectoryLine;
	FirstPersonController playerController;

	void Awake(){

	}

	void Start () {
		hasKey = new bool[3];
		flashlight = GetComponentsInChildren<Light> ();
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera");
		audioSource = GetComponent<AudioSource>();
		armsAnim =  GameObject.FindGameObjectWithTag("Arms").GetComponent<Animator>();
		gun = GetComponentInChildren<GunController>();
		cameraAnim = mainCam.GetComponent<Animator>();
		cameraSanity = mainCam.GetComponent<MotionBlur>();
		playerController = GetComponent<FirstPersonController>();
		trajectoryLine = GetComponentInChildren<ProjectileThrower> ();
	}

	void Update () {
		isLoud = playerController.isPlayerRunning(); //update sound state for zombie's hearing detection
		Regenerate ();

		PauseControl ();
		ThrowControls ();
		FlashlightControl ();
		ReloadControl ();
		AimControl ();
		GunControl ();
		HealingControls ();
		Damaged ();
	}

	void Regenerate(){
		sanityTimer += Time.deltaTime;
		regenTimer += Time.deltaTime;
		//REGENERATION--sanity
		if(sanityTimer >= 5 && !regenSanity){ //if sanity is not reduced in the last 5-7s, regenerate sanity
			regenSanity = true;
			sanityTimer = 0;
		}
		if (regenSanity) updateSanity (false, 0.0025f);

		//REGENERATION--health
		if(regenTimer >= 5 && alive){ //regenerate 1 hp/5 seconds
			if(playerHealth < 100){
				playerHealth++;
				HUDManager.UpdateHealth (playerHealth);
				regenTimer = 0;
			}
		}
	}

	void PauseControl(){ //PAUSE GAME
		if(Input.GetKeyDown(KeyCode.Escape) && mainCam.activeSelf){
			HUDManager.TogglePause();
		}
	}

	void ThrowControls(){ //THROWING
		if (Input.GetKeyDown (KeyCode.Q) && hasBait && hasGun) { //SWITCH GEAR---GUN/BAIT
			readyThrow = !readyThrow;
			gunActive = !readyThrow;
			HUDManager.ToggleCrosshair (gunActive);
			throwWindUp = false;
			armsAnim.SetBool ("throwWindUp", throwWindUp);
			armsAnim.SetBool ("readyThrow", readyThrow);
			armsAnim.SetBool ("cancelWindUp", true);
			cancelThrow = true;
			trajectoryLine.setReadyThrow (false);
			trajectoryLine.cancelThrow ();
		}
		if (Input.GetButtonDown ("Fire1") && readyThrow) { //THROW-hold LEFT CLICK; wind up animation--ready to throw
			cancelThrow = false;
			armsAnim.SetBool ("cancelWindUp", false);
			throwWindUp = true;
			armsAnim.SetBool ("throwWindUp", throwWindUp);
			trajectoryLine.setReadyThrow (true);
		}
		if (Input.GetButtonUp ("Fire2") && readyThrow) { //THROW-RIGHT CLICK; cancel wind up animation
			armsAnim.SetBool ("cancelWindUp", true);
			throwWindUp = false;
			armsAnim.SetBool ("throwWindUp", throwWindUp);
			cancelThrow = true;
			trajectoryLine.setReadyThrow (false);
			trajectoryLine.cancelThrow ();
		}
		if (Input.GetButtonUp ("Fire1") && readyThrow && throwWindUp) { //THROW-release LEFT CLICK; throw
			throwWindUp = false;
			readyThrow = !readyThrow;
			armsAnim.SetBool ("throwWindUp", throwWindUp);
			armsAnim.SetBool ("readyThrow", readyThrow);
			gunActive = true;
			HUDManager.ToggleCrosshair (true);
		}
	}

	void FlashlightControl(){ //FLASHLIGHT
		if(Input.GetKeyDown(KeyCode.F) && hasFlashlight){
			if(flashlight[0].enabled){
				audioSource.PlayOneShot(flashlightOn);
				flashlight[0].enabled = false;
				flashlight[1].enabled = false;
			} else {
				audioSource.PlayOneShot(flashlightOff);
				flashlight[0].enabled = true;
				flashlight[1].enabled = true;
			}
		}
	}

	void ReloadControl(){ //RELOAD
		if(Input.GetKeyDown(KeyCode.R) && gun.currentAmmo < 6 && gun.maxAmmo > 0 && hasGun && gunActive && !isReloading){
			if(isAiming){
				armsAnim.SetBool("aiming", false);
				cameraAnim.SetBool("zoom", false);
				isAiming = false;
			}
			armsAnim.SetBool("aiming", false);
			cameraAnim.SetBool("zoom", isAiming);
			HUDManager.StartReloadAnim ();
			armsAnim.SetTrigger("reload");
			isReloading = true;
			HUDManager.ToggleCrosshair (false);
		}
	}

	void AimControl(){
		if(Input.GetButtonDown ("Fire2") && hasGun && gunActive){
			armsAnim.SetBool("aiming", isAiming);
			cameraAnim.SetBool("zoom", isAiming);
			isAiming = !isAiming;
			HUDManager.ToggleCrosshair (isAiming);
		}
	}

	void GunControl(){ //FIRE GUN
		shotTimer += Time.deltaTime;
		if(Input.GetButtonDown ("Fire1") && shotTimer >= timeBetweenShots && Time.timeScale != 0 && gun.currentAmmo > 0 && hasGun && gunActive)
		{
			gun.Fire();
			armsAnim.SetTrigger("fire");
			HUDManager.WidenCrosshair ();
			shotTimer = 0;
			isLoud = true;
		}
	}

	void HealingControls(){
		if(isHealed){
			if(playerHealth < 100){
				playerHealth++;
				HUDManager.UpdateHealth (playerHealth);
				healingReceived--;
			}
			if(healingReceived == 0 || playerHealth == 100){
				healingReceived = 0;
				isHealed = false;
			}
		}
		if(Input.GetKey(KeyCode.Alpha1) && healthKitCount > 0 && playerHealth < 100){ //hold '1' to heal
			healthKitTimer += Time.deltaTime;
			HUDManager.UpdateHealthKitTimer (true, healthKitTimer / 3f);
			if(!isHealing) armsAnim.SetTrigger ("startHeal");
			gunActive = false;
			isAiming = false;
			throwWindUp = false;
			cancelThrow = true;
			armsAnim.SetBool("aiming", isAiming);
			cameraAnim.SetBool("zoom", false);
			armsAnim.SetBool ("throwWindUp", throwWindUp);
			armsAnim.SetBool ("readyThrow", readyThrow);
			armsAnim.SetBool ("cancelWindUp", true);
			isHealing = true;
			if(healthKitTimer >= 3){ //after holding down '1' for 3s, heal
				healthKitTimer = 0;
				healthKitCount--;
				HUDManager.UpdateHealthKitTimer (false, 0);
				HUDManager.UpdateHealthKit (healthKitCount);
				armsAnim.SetTrigger ("stopHeal");
				gunActive = true;
				isHealing = false;
				healingReceived += 25;
				isHealed = true;
			}
		}
		if(Input.GetKeyUp(KeyCode.Alpha1) && isHealing){
			armsAnim.SetTrigger ("stopHeal");
			HUDManager.UpdateHealthKitTimer (false, 0);
			healthKitTimer = 0;
			gunActive = true;
			isHealing = false;
		}
	}

	void Damaged(){
		if(damageTaken > 0 && playerHealth > 0){
			playerHealth -= 2;
			HUDManager.UpdateHealth (playerHealth);
			damageTaken -= 2;
			if(playerHealth <= 0){
				alive = false;
				HUDManager.GameOver();
				gameManager.EndGame ();
				playerController.enabled = false; //disable player controller
			}
		}
	}

	public void TakeDamage (float amount)
	{
		isDamaged = true;
		damageTaken += amount;
		audioSource.PlayOneShot(hurtSound);
		updateSanity(true, amount); //taking damage reduces sanity
		HUDManager.FlashDamage (isDamaged);
	}

	public void PickupItem(string item){ //PICK UP based on CATEGORY
		string[] itemName = item.Trim().Split(' ');
		switch(itemName[0]){
		case "Flashlight":
			HUDManager.DisableDot ();
			hasFlashlight = true;
			armsAnim.SetTrigger ("enableFlashlight");
			gameManager.RemoveHallBlocker (0);
			gameManager.JumpscareBuildup ();
			HUDManager.UpdateObjective ("");
			HUDManager.ShowTutorial ("[F] TO TOGGLE FLASHLIGHT", 0);
			HUDManager.ShowTutorial ("", 1);
			break;
		case "Weapon":
			hasGun = true;
			HUDManager.TogglePlayerUI ();
			HUDManager.ToggleCrosshair (true);
			armsAnim.SetTrigger ("enableGun");
			gunActive = true;
			gameManager.RemoveHallBlocker (1);
			gameManager.RemoveHallBlocker (2);
			HUDManager.UpdateObjective ("FIND THE EXIT");
			gun.AmmoPickup ();
			HUDManager.ShowTutorial ("[R] TO RELOAD", 0);
			HUDManager.ShowTutorial ("", 1);
			break;
		case "Ammo":
			gun.AmmoPickup ();
			HUDManager.DisplayText ("+3 AMMO", 2.5f);
			break;
		case "Bait":
			hasBait = true;
			HUDManager.ToggleBaitUI (true);
			break;
		case "Key":
			if (itemName [1] == "Red"){
				hasKey [0] = true;
				HUDManager.ShowKeyUI (0);
			}
			else if (itemName [1] == "Green"){
				hasKey [1] = true;
				HUDManager.ShowKeyUI (1);
			}
			else if (itemName [1] == "Yellow"){
				hasKey [2] = true;
				HUDManager.ShowKeyUI (2);
			}	
			HUDManager.UpdateObjective ("FIND THE " + itemName[1] + " DOOR");
			break;
		case "HealthKit":
			healthKitCount++;
			HUDManager.UpdateHealthKit (healthKitCount);
			break;
		}
	}

	public void ToggleFPSController(){
		playerController.enabled = !playerController.enabled;
	}

	public bool IsMakingSounds(){
		return isLoud;
	}

	public void FinishReload(){
		isReloading = false;
	}

	public void updateSanity(bool reduce, float value){ //INCREASE/DECREASE CAMERA BLUR (SANITY)
		if(reduce){
			if(cameraSanity.blurAmount < 0.92f) cameraSanity.blurAmount += value;
			if (cameraSanity.blurAmount >= 0.6f) cameraSanity.extraBlur = true;
			if (cameraSanity.blurAmount > 0.92f) cameraSanity.blurAmount = 0.92f;
			regenSanity = false; //disable sanity regeneration
			sanityTimer = 0; //reset timer before sanity regen is enabled
		} else {
			if(cameraSanity.blurAmount > 0.2f) cameraSanity.blurAmount -= value;
			if (cameraSanity.blurAmount < 0.6f) cameraSanity.extraBlur = false;
			if (cameraSanity.blurAmount < 0.2f) cameraSanity.blurAmount = 0.2f;
		}
	}

	public void ThrowBait(bool isReleased){ //FUNCTION CALL TO THROWER OBJECT
		if(isReleased){
			if(!cancelThrow){ //if transition is caused by throwing
				trajectoryLine.ThrowProjectile ();
				hasBait = false;
				HUDManager.ToggleBaitUI (false);
			} else { //if transition is caused by Q or RIGHT CLICK (switch weapons/cancel throw)
				trajectoryLine.setReadyThrow (false);
				trajectoryLine.cancelThrow ();
				cancelThrow = false;
			}	
		} else{
			trajectoryLine.setReadyThrow (true);
		}
	}

	public void updateEnemyChasingCount(bool increment){
		if (increment)
			enemyChasingCount++;
		else
			enemyChasingCount--;
	}

	public int getEnemyChasingCount(){
		return enemyChasingCount;
	}

	public bool isFlashlightOn(){
		return flashlight[0].enabled;
	}

	public bool hasLight(){
		return hasFlashlight;
	}

	public bool hasCup(){
		return hasBait;
	}

	public bool hasWeapons(){
		return hasGun;
	}

	public bool hasDoorKey(int index){
		return hasKey [index];
	}

	public bool isAlive(){
		return alive;
	}

	public void FinishGame(){
		StartCoroutine (ShowEnding ());
	}

	IEnumerator ShowEnding(){
		gameManager.EndGame ();
		yield return new WaitForSeconds(1.5f);
		playerController.enabled = false;
	}

}
