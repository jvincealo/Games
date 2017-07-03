using UnityEngine;
using System.Collections;

public class ObjectiveManager : MonoBehaviour {
	public GameManager gameManager;
	public GameUI HUDManager;
	public string objectiveText;
	public bool isNotice;
	public bool isTutorial;	

	void OnTriggerEnter(Collider col){
		Player player = col.gameObject.GetComponent<Player> ();
		if(col.tag == "Player"){
			if(isNotice){
				HUDManager.UpdateObjective (objectiveText);
			} else if(isTutorial){
				if(player.hasWeapons()){
					HUDManager.ToggleStealthTutorial ();
					GameObject.Destroy (transform.parent.gameObject);
				}
			} else {
				if(gameObject.tag == "JumpscareBuildup"){
					gameManager.JumpscareBuildup ();
					GameObject.Destroy (gameObject);
				} else {
					gameManager.ChangeMusic ();
					GameObject.Destroy (gameObject);
				}
			}
		}
	}
}
