using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {
	public GameUI gameUI;
	public GameObject cutsceneCam;
	public Image crosshairDot;

	public GameObject[] zombieSpawn;
	public GameObject[] hallwayBlocker;

	GameObject mainCamera;
	GameObject playerObject;
	Player player;

	AudioSource bgAudio;
	AudioSource panicAudio;
	AudioSource jumpscareAudio;
	public AudioClip gameOverTheme;
	public AudioClip wakeUpTheme;
	public AudioClip panicMusic;
	public AudioClip jumpscareBuildup;
	public AudioClip jumpscareSound;
	bool jumpscareOngoing;
	bool introFinished;

	void Start () {
		bgAudio = GetComponent<AudioSource> ();
		playerObject = GameObject.FindGameObjectWithTag("Player");
		player = playerObject.GetComponent<Player> ();
		playerObject.GetComponent<FirstPersonController>().enabled = false;
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		panicAudio = GetComponentsInChildren<AudioSource>()[1];
		jumpscareAudio = GetComponentsInChildren<AudioSource>()[2];
		crosshairDot.enabled = false;
		mainCamera.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
		bgAudio.Stop ();
		bgAudio.PlayOneShot (wakeUpTheme);
	}

	void Update(){
		if(mainCamera.activeSelf){ //start scanning states after initial cutscene (waking up)
			if(player.isAlive()){
				if(player.getEnemyChasingCount() > 0){ //play panic sound
					if (!panicAudio.isPlaying) {
						panicAudio.volume = 1;
						panicAudio.PlayOneShot (panicMusic);
					}
				} else {
					if (panicAudio.isPlaying) {
						panicAudio.volume -= Time.deltaTime;
						if(panicAudio.volume <= 0) panicAudio.Stop ();
					}
				}

				StartCoroutine(CheckZombieSpawn());
			} else {
				bgAudio.volume += Time.deltaTime/8;
				panicAudio.volume -= Time.deltaTime;
				jumpscareAudio.volume -= Time.deltaTime;
			}
		}	
	}

	public void SwitchToGame(){ //cutscene finished
		crosshairDot.enabled = true;
		mainCamera.SetActive(true);
		cutsceneCam.SetActive(false);
		playerObject.GetComponent<FirstPersonController>().enabled = true;
		playerObject.GetComponent<Player>().enabled = true;
		gameUI.ShowTutorial ("[W][A][S][D] TO WALK", 0);
		gameUI.ShowTutorial ("[LEFT SHIFT] TO SPRINT", 1);
	}

	public void ChangeMusic(){
		StartCoroutine (FadeOutSound (bgAudio, true));
	}

	IEnumerator FadeOutSound(AudioSource audio, bool playNext){
		while(audio.volume > 0){
			audio.volume -= 0.02f;
			yield return new WaitForSeconds(0.02f);
		}
		audio.Stop ();
		if(playNext){
			audio.Play ();
			audio.volume = 1;
		} 
	}

	public void RemoveHallBlocker(int index){
		GameObject.Destroy(hallwayBlocker [index]);
	}

	public void StartJumpscare(){
		jumpscareAudio.PlayOneShot (jumpscareSound);
	}

	public void JumpscareBuildup(){
		jumpscareAudio.PlayOneShot (jumpscareBuildup);
	}

	IEnumerator CheckZombieSpawn(){
		if(player.hasWeapons()){ //player has weapons, spawn combat tutorial zombie
			SpawnZombie (0);
		}
		if(player.hasDoorKey(0)){ //player has red key, spawn the 3 zombies
			for(int i=1; i<zombieSpawn.Length; i++){
				yield return new WaitForSeconds(0.66f);
				SpawnZombie (i);
			}
		}
	}

	void SpawnZombie(int index){
		zombieSpawn [index].SetActive (true);
	}

	public void EndGame(){
		bgAudio.volume = 1;
		bgAudio.Stop ();
		bgAudio.PlayOneShot (gameOverTheme);
	}

}
