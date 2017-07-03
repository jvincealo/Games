using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorScript : MonoBehaviour {
	public Player player;
	public GameUI HUDManager;
	public AudioClip doorOpen;
	public AudioClip doorClose;
	public AudioClip doorLock;
	public AudioClip doorUnlock;
//	public AudioClip doorCreak;
	public bool specialDoor;
	public int doorColor; //0,1,2 = red,blue,yellow

	bool isUnlocked;
	bool withinPlayerRange;
	bool endGame;
	Animator doorAnim;
	AudioSource doorAudio;

	// Use this for initialization
	void Start () {
		doorAnim = GetComponent<Animator>();
		doorAudio = GetComponent<AudioSource>();
		isUnlocked = !specialDoor;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.E) && withinPlayerRange){
			if(specialDoor && !isUnlocked){
				if(player.hasDoorKey(doorColor)){
					HUDManager.DisplayText ("Door unlocked.", 5);
//					doorAudio.PlayOneShot(doorUnlocked);
					Debug.Log("Door Unlocked!");
					isUnlocked = true;
					doorAudio.PlayOneShot (doorUnlock);
					if(doorColor == 2){
						player.FinishGame ();
						HUDManager.EndGame ();
						doorAnim.SetTrigger("interact");
						HUDManager.UpdateObjective ("SURVIVE");
						withinPlayerRange = false;
						HUDManager.ToggleDoorLabel(false, false);
						endGame = true;
					} else {
						HUDManager.UpdateObjective("FIND THE EXIT");
					}
				} else {
					HUDManager.DisplayText ("Locked. Find the key", 5);
//					doorAudio.PlayOneShot(doorLocked);
					Debug.Log("Door is Locked.");
					doorAudio.PlayOneShot (doorLock);
				}
			} else {
				Debug.Log("Current Angle is " + transform.localEulerAngles.y);
				Debug.Log("Opening/Closing Door...");
				HUDManager.ToggleDoorLabel(false, false);
				
				if(transform.localEulerAngles.y >= 269){
					doorAudio.PlayOneShot(doorClose);
				} else {
					doorAudio.PlayOneShot(doorOpen);
				}
				doorAnim.SetTrigger("interact");
			}

		}
	}

	void OnTriggerStay(Collider other){
		if(other.tag == "Player" && !endGame){
			//calculate first if player is facing the door
			Vector3 currentPos = other.gameObject.transform.position;
			Vector3 targetDir = transform.position;
			currentPos.y = 0;
			targetDir.y = 0;
			Vector3 direction = targetDir - currentPos;
//			Debug.Log(Vector3.Angle(direction, other.gameObject.transform.forward));
			if ((Vector3.Dot(direction, other.gameObject.transform.forward) >= 0) && Vector3.Angle(direction, other.gameObject.transform.forward) < 60) {
				withinPlayerRange = true;
				if(doorColor == 2) HUDManager.ToggleDoorLabel(true, true);
				else HUDManager.ToggleDoorLabel(true, false);
			} else {
				withinPlayerRange = false;
				HUDManager.ToggleDoorLabel(false, false);
			}
		}	
	}

	void OnTriggerExit(Collider other){
		withinPlayerRange = false;
		HUDManager.ToggleDoorLabel(false, false);
	}

}
