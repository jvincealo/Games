using UnityEngine;
using System.Collections;

public class Debris : MonoBehaviour {
	public float debrisForce; //speed of flying debris
	public float rotationSpeed;
	public bool enableSpin;
	public bool destroyOnContact;
	public bool enableBlink;

	float blinkInterval = 2.5f;
	float blinkTimer;
	Rigidbody2D rBody;
	BoxCollider2D boxCol;
	SpriteRenderer lightningSprite;

	void Start(){
		if (!gameObject.isStatic) {
			rBody = GetComponent<Rigidbody2D> ();
			rBody.AddForce (new Vector2 (-1 * debrisForce, 0));
		}
		if (enableBlink){
			boxCol = GetComponent<BoxCollider2D> ();
			lightningSprite = GetComponent<SpriteRenderer> ();
		}
	}

	void FixedUpdate(){
		if(enableSpin)
			transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time. fixedDeltaTime);
		if(enableBlink){
			blinkTimer += Time.deltaTime;
			if (blinkTimer >= blinkInterval){
				boxCol.enabled = !boxCol.enabled;
				lightningSprite.enabled = !lightningSprite.enabled;
				blinkTimer = 0;
			}
		}
	}
		
		
}
