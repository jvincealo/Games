using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {
	public Sprite[] bulletSprite;
	List<GameObject> bulletObjectPool;
	List<Bullet> bulletPool;
	List<GameObject> terrainPool;
	public GameObject bullet;
	public GameObject[] terrain;
	public int bulletPoolAmount;
	public int terrainPoolAmount;
	public GameObject wipeout;

	void Start(){
		bulletObjectPool = new List<GameObject> ();
		terrainPool = new List<GameObject> ();
		bulletPool = new List<Bullet> ();
		for (int i = 0; i < bulletPoolAmount; i++) { //instantiate pool of bullets
			GameObject temp = Instantiate (bullet) as GameObject;
			temp.SetActive (false);
			bulletObjectPool.Add (temp);
			bulletPool.Add(temp.GetComponent<Bullet> ());
		}

		for(int i=0; i<terrainPoolAmount; i++){
			for(int j=0; j<terrain.Length; j++){ //instantiate pool of terrains
				GameObject temp2 = Instantiate (terrain [j]) as GameObject;
				temp2.SetActive (false);
				terrainPool.Add (temp2);
			}
		}

		wipeout = Instantiate (wipeout) as GameObject;
		wipeout.SetActive (false);
	}

	public GameObject GetBullet(int level){
		for(int i = 0; i < bulletPoolAmount; i++){ //get bullet from pool
			if(!bulletObjectPool[i].activeInHierarchy){
				bulletPool [i].SetBulletLevel (bulletSprite [level], level);
				return bulletObjectPool [i];
			}
		}
		//pool amount is too small; expand
		GameObject temp = Instantiate (bullet) as GameObject;
		bulletObjectPool.Add (temp);
		bulletPool.Add(temp.GetComponent<Bullet> ());
		bulletPoolAmount++;
		temp.GetComponent<Bullet> ().SetBulletLevel (bulletSprite [level], level);
		return temp;
	}

	public GameObject GetTerrain(float xPos){
		bool returnTerrain;
		while(true){
			int index = Random.Range (0, (terrainPoolAmount * terrain.Length) - 1);
			if (!terrainPool [index].activeInHierarchy){ //if terrain is disabled
				terrainPool[index].SetActive(true);
				terrainPool [index].transform.position = new Vector3 (xPos, 0, 0);
				return terrainPool [index];
			}
		}
	}

	public void DisableTerrain(GameObject previous){
		if (terrainPool.Contains (previous))
			previous.SetActive (false); //if terrain to remove is in object pool, disable
		else
			GameObject.Destroy (previous);
	}

	public GameObject GetWipeout(){
		wipeout.SetActive (true);
		return wipeout;
	}
}
