using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	SpriteRenderer spriteImage;
	public int bulletDamage = 5;

	void Awake(){
		spriteImage = GetComponent<SpriteRenderer> (); //sprite renderer is accessed before object is enabled hence place in Awake() and not Start()
	}


	public void SetBulletLevel(Sprite sprite, int level){
		spriteImage.sprite = sprite;
		bulletDamage *= level + 1;
	}

	void OnCollisionEnter2D(Collision2D col){
		gameObject.SetActive (false);
		if(col.gameObject.tag == "Enemy"){
			col.gameObject.GetComponent<Enemy> ().TakeDamage (bulletDamage);
		}
	}
}
