using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
	Player player;
	public Image crosshairDot;
	public GameObject crosshair;
	public GameObject stealthTutorial;
	public Text[] tutorialText;
	public GameObject controlPanel;
	public GameObject pausePanel;
	public Button resumeBtn;
	public Button controlsBtn;
	public Button exitBtn;
	public Image damageImage;
	public Image healthBar;
	public Image[] keyUI;
	public GameObject gunPanel;
	public GameObject baitPanel;
	public Image baitImage;
	public Text maxAmmo;
	public Image currentAmmo;
	public Text doorLabel;
	public Image fadeImage;
	public Text gameOverText;
	public Text endGameText;
	public Text objectiveText;
	public Text noticeText;
	public Text healthKitText;
	public Image healthKitSlider;
	Animator reloadAnim;
	Animator crosshairAnim;

	bool startFade;
	bool isDamaged;
	bool stealthFade;
	float noticeTimer;
	bool noticeStartTimer;
	float noticeMaxTime;
	bool gameOver;
	bool endGame;
	float alphaThreshold;
	float alphaValue;
	float tutorialTimer;
	float tutorialEnd;
	bool startTutorialTimer;

	AudioSource gameAudio;
	public AudioClip stealthKillSound;

	void Start(){
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		reloadAnim = gunPanel.GetComponent<Animator> ();
		crosshairAnim = crosshair.GetComponent<Animator> ();
		gameAudio = GetComponent<AudioSource>();
	}

	void Update(){
		if(noticeStartTimer){
			noticeTimer += Time.deltaTime;
			if(noticeTimer >= noticeMaxTime){
				noticeTimer = 0;
				noticeStartTimer = false;
				noticeText.enabled = false;
			}
		}
		if(startFade){
			
			if (fadeImage.color.a < alphaThreshold) {
				fadeImage.color = Color.Lerp(fadeImage.color, new Color(0, 0, 0, alphaValue), Time.deltaTime/1000);
			} else {
				if (gameOverText.color.a < 1 && gameOver) {
					if(Time.timeScale > .25f) Time.timeScale -= Time.deltaTime/2;
					gameOverText.color = Color.Lerp(gameOverText.color, new Color(40, 10, 10, 255), Time.deltaTime/500);
				}
				if (endGameText.color.a < 1 && endGame) {
					endGameText.color = Color.Lerp(endGameText.color, new Color(255, 255, 255, 255), Time.deltaTime/500);
				}
			}
		}

		if(stealthFade && !endGame && !gameOver){
			if (fadeImage.color.a > 0.0001f) {
				fadeImage.color = Color.Lerp(fadeImage.color, new Color(75/255f, 0, 0, 0), Time.deltaTime/3);
			}
		}

		if(isDamaged){
			damageImage.color = new Color(200, 20, 20, 255);
		} else {
			damageImage.color = Color.Lerp(damageImage.color, new Color(200, 20, 20, 0), Time.deltaTime*20);
		}
		isDamaged = false;

		if(startTutorialTimer){
			tutorialTimer += Time.deltaTime;
			if(tutorialTimer >= tutorialEnd){
				for (int i = 0; i < tutorialText.Length; i++)
					tutorialText [i].enabled = false;
			}
		}
	}

	public void DisableDot(){
		crosshairDot.enabled = false;
	}

	public void WidenCrosshair(){
		crosshairAnim.SetTrigger ("fire");
	}

	public void ToggleCrosshair(bool value){
		crosshair.SetActive (value);
	}

	public void ToggleStealthTutorial(){
		stealthTutorial.SetActive (!stealthTutorial.activeSelf);
		Cursor.visible = !Cursor.visible;
		Cursor.lockState = (Cursor.visible) ? CursorLockMode.None : CursorLockMode.Locked;
		player.ToggleFPSController();
		Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
		if(!stealthTutorial.activeSelf){
			ShowTutorial ("[LEFT MB] TO FIRE", 0);
			ShowTutorial ("[RIGHT MB] TO AIM", 1);
		}
	}

	public void ShowTutorial(string text, int index){
		startTutorialTimer = true;
		tutorialTimer = 0;
		tutorialEnd = 5f;
		tutorialText [index].enabled = true;
		tutorialText [index].text = text;
	}

	public void TogglePause(){
		Cursor.visible = !Cursor.visible;
		Cursor.lockState = (Cursor.visible) ? CursorLockMode.None : CursorLockMode.Locked;
		player.ToggleFPSController();
		pausePanel.SetActive(!pausePanel.activeSelf);
		Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
	}

	public void ToggleControlsUI(){
		controlPanel.SetActive(!controlPanel.activeSelf);
		resumeBtn.interactable = !resumeBtn.interactable;
		controlsBtn.interactable = !controlsBtn.interactable;
		exitBtn.interactable = !exitBtn.interactable;
	}

	public void FlashDamage(bool value){
		isDamaged = value;
	}

	public void UpdateHealth(float health){
		healthBar.fillAmount = health / 100f;
	}

	public void StartReloadAnim(){
		reloadAnim.SetTrigger ("reload");
	}

	public void UpdateAmmo(int current, int max){
		maxAmmo.text = "" + max;
		currentAmmo.fillAmount = current / 6f;
	}

	public void ToggleBaitUI(bool hasBait){
		if(hasBait){
			baitImage.color = new Color (255, 255, 255, 255);
		} else {
			baitImage.color = new Color (255, 255, 255, 160/255f);
		}
	}

	public void TogglePlayerUI(){
		healthBar.enabled = true;
		baitPanel.SetActive (true);
		gunPanel.SetActive (true);
	}

	public void ShowKeyUI(int index){
		keyUI [index].enabled = true;
	}

	public void ToggleDoorLabel(bool value, bool yellowDoor){
		if(yellowDoor){
			doorLabel.text = "PRESS [E] TO LEAVE";
			doorLabel.color = new Color (100/255f, 0, 0, 1);
		} else {
			doorLabel.color = Color.white;
			doorLabel.text = "PRESS [E] TO OPEN/CLOSE DOOR";
		}
		doorLabel.enabled = value;
	}

	public void StealthFlashImage(){
		gameAudio.PlayOneShot(stealthKillSound);
		fadeImage.color = new Color(75/255f, 0, 0, 1);
		stealthFade = true;
	}

	public void UpdateObjective(string text){
		objectiveText.text = text;
	}

	public void DisplayText(string text, float displayTime){
		noticeMaxTime = displayTime;
		noticeText.text = text;
		noticeText.enabled = true;
		noticeTimer = 0;
		noticeStartTimer = true;
	}

	public void UpdateHealthKit(int count){
		if (!healthKitSlider.gameObject.activeSelf){
			healthKitSlider.gameObject.SetActive (true);
			ShowTutorial ("[HOLD] [1] TO HEAL", 0);
			ShowTutorial ("", 1);
		}
		healthKitText.text = "" + count;
	}

	public void UpdateHealthKitTimer(bool enable, float timer){
		healthKitSlider.enabled = enable;
		healthKitSlider.fillAmount = timer;
	}

	public void GameOver(){
		alphaThreshold = 1;
		alphaValue = 255;
		fadeImage.color = new Color(0, 0, 0, 0);	
		gameOver = true;
		startFade = true;
	}

	public void EndGame(){
		alphaThreshold = .8125f;
		alphaValue = 208;
		fadeImage.color = new Color(0, 0, 0, 0);	
		endGame = true;
		startFade = true;
	}

	public void ExitToMainMenu(){
		SceneManager.LoadScene("MainMenu");
	}

}
