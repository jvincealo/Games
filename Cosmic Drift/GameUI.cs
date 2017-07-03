using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
	public AudioSource gameAudio;
	public Sprite[] skillIcon;
	public Text coinText;
	public Text distanceText;
	public Image ammoCooldown;
	public Text ammo;
	public Image skill;
	public Image skillCooldown;
	public Text skillAmmo;
	public GameObject endPanel;
	public Text scoreDistText;
	public Text scoreCoinText;
	public Text highscoreDistText;
	public Text highscoreCoinText;
	public Text newHighScoreNotice;

	void Start(){
		gameAudio = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioSource>();
	}


	public void UpdateDistance(float dist){ 
		distanceText.text = dist.ToString("F2") + " km"; //F2 - 2 decimal places float
	}

	public void UpdateCoins(int coins){
		coinText.text = "" + coins;
	}

	public void UpdateAmmo(int ammo, bool isBullet){
		if (isBullet)
			this.ammo.text = "" + ammo;
		else
			skillAmmo.text = "" + ammo;
	}

	public void UpdateCooldown(float cooldown, bool isBullet){
		if (isBullet)
			ammoCooldown.fillAmount = 1 - cooldown;
		else 
			skillCooldown.fillAmount = 1 - cooldown;
	}

	public void UpdateSkill(int skillNum){
		if (skillAmmo.text.Equals ("")){
			skill.color = new Color (1, 1, 1, 1);
			skill.type = Image.Type.Simple;
		}
		skill.sprite = skillIcon [skillNum];
	}

	public void RestartGame(){
		gameAudio.Stop ();
		gameAudio.Play ();
		SceneManager.LoadSceneAsync ("Main");
	}
		
	public void ShowEndScreen(float dist, int coins, float highscoreDist, int highscoreCoins){
		endPanel.SetActive (true);
		scoreCoinText.text = "" + coins;
		scoreDistText.text = "" + dist.ToString("F2");
		if (dist > highscoreDist){
			newHighScoreNotice.enabled = true;	
			highscoreCoinText.text = "" + coins;
			highscoreDistText.text = "" + dist.ToString("F2");
		} else {
			newHighScoreNotice.enabled = false;
			highscoreCoinText.text = "" + highscoreCoins;
			highscoreDistText.text = "" + highscoreDist.ToString("F2");
		}
	}
}
