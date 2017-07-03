using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public float enemySpeed;
	public int health = 5;

	Rigidbody2D rBody;
	Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
		rBody = GetComponent<Rigidbody2D> ();
		rBody.AddForce (new Vector2 (-1 * enemySpeed, 0));
	}

	public void TakeDamage(int damage){
		health -= damage;
		if(health <= 0)
			Explode ();
	}

	public void Explode(){
		GetComponent<Collider2D> ().enabled = false;
		anim.SetTrigger ("explode");
	}
}
