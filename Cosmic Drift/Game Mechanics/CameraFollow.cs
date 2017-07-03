using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Player player;
	float xOffset;

	void Start (){
		xOffset = transform.position.x - player.transform.position.x;
	}

	void Update () {
		if(player.isAlive())
			transform.position = new Vector3(player.transform.position.x + xOffset, transform.position.y, transform.position.z);
	}
}
