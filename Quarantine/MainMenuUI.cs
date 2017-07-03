using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuUI : MonoBehaviour {
	AudioSource soundEffects;
	public Text gameTitle;
	public GameObject controlsUI;
	public AudioSource musicSource;
	public AudioClip startGameSound;
	public Image fadeImage;
	Text loadText;
	Image loadImage;

	bool startFade;

	void Start(){
		soundEffects = GetComponent<AudioSource>();
		loadText = fadeImage.gameObject.GetComponentInChildren<Text> ();
		loadImage = fadeImage.transform.GetChild (1).GetComponent<Image> ();
	}

	void Update(){
		if(startFade){
			fadeImage.color = Color.Lerp(fadeImage.color, new Color(0,0,0, 255), Time.deltaTime/500);
			if (fadeImage.color.a >= 1) {
				loadText.enabled = true;
				loadImage.enabled = true;
				Debug.Log("Starting Game...");
				startFade = false;
				musicSource.Stop();
				StartCoroutine("LoadingGame");
			}
		}
	}

	IEnumerator LoadingGame(){
		yield return new WaitForSeconds(3);
		SceneManager.LoadSceneAsync ("StartGame", LoadSceneMode.Single);
	}

	public void StartGame(){
		Cursor.lockState = CursorLockMode.Locked;
		startFade = true;
		gameTitle.enabled = true;
		controlsUI.SetActive(false);
		soundEffects.PlayOneShot(startGameSound);
	}

	public void ShowControls(){
		gameTitle.enabled = false;
		controlsUI.SetActive(true);
	}

	public void CloseControls(){
		gameTitle.enabled = true;
		controlsUI.SetActive(false);
	}

	public void ExitGame(){
		Application.Quit ();
	}

}
