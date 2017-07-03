using UnityEngine;
using System.Collections;

public class BlinkingLight : MonoBehaviour {
	float blinkCounter = 0;
	float timer; //no light timer
	float blinkTimer; //blink timer
	bool resetTimer = true;
	bool stopBlink = false;
	public float blinkInterval = 0.0625f;
	public float noLightInterval = 3;

	Light blinkLight;

	// Use this for initialization
	void Start () {
		blinkLight = GetComponent<Light>();
		blinkLight.enabled = !blinkLight.enabled;
	}
	
	// Update is called once per frame
	void Update () {
		if (resetTimer) {
			if(blinkCounter <= 5){ //5th count = light is off
				blinkLight.enabled = !blinkLight.enabled;
				blinkCounter++;
				resetTimer = false; //after every blink, count to blink interval
			} else { //after 3 blinks, reset counter
				blinkCounter = 0;
				resetTimer = false;
				stopBlink = true;
			}
		} else { //duration of every on/off
			if(!stopBlink){
				blinkTimer += Time.deltaTime;
				if (blinkTimer >= blinkInterval){
					resetTimer = true;
					blinkTimer = 0;
				}
			}
		}

		if(blinkCounter == 0) timer += Time.deltaTime; //start no light counter after 3 blinks
		if(timer >= noLightInterval){
			resetTimer = true;
			stopBlink = false;
			timer = 0;
		}

	}
}
