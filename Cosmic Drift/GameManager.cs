using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	AsyncOperation async;
	Dictionary<string, AudioClip> sfx;
	AudioSource gameAudio;
	public AudioClip gun;
	public AudioClip[] explosion; //0-enemy, 1-smoke, 2-player
	public AudioClip coin;

	//powerups / skills
	public AudioClip[] skill; //0-dash, 1-wipeout
	public AudioClip booster;
	public AudioClip magnet;

	void Start () {
		DontDestroyOnLoad(gameObject);

		gameAudio = GetComponent<AudioSource> ();
		sfx = new Dictionary<string, AudioClip> ();
		AddAudio ();
		async = SceneManager.LoadSceneAsync ("Main"); //load game
		async.allowSceneActivation = false;
	}

	void Update () {
		if(Input.touchCount > 0 && Input.GetTouch (0).phase != TouchPhase.Ended && Input.GetTouch (0).phase != TouchPhase.Canceled) 
//		if(Input.GetKey(KeyCode.Space)) //pc testing
			StartGame ();
	}

	public void StartGame(){
		async.allowSceneActivation = true;
	}

	void AddAudio(){
		sfx.Add ("enemy", explosion [0]);
		sfx.Add("debris", explosion[1]);
		sfx.Add("player", explosion[2]);
		sfx.Add ("gun", gun);
		sfx.Add ("dash", skill [0]);
		sfx.Add ("wipeout", skill [1]);
		sfx.Add ("booster", booster);
		sfx.Add ("magnet", magnet);
		sfx.Add ("coin", coin);
	}

	public void PlaySfx(string name){
		AudioClip temp;
		sfx.TryGetValue (name, out temp);
		gameAudio.PlayOneShot (temp);
	}

}
