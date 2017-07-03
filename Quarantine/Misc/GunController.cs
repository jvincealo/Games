using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {
	public GameUI HUDManager;
	public float gunDamage = 35f;
	public int maxAmmo = 24;
	public int currentAmmo = 6;

	public AudioClip fireSound;
	public AudioClip reloadSound;

	bool isFired;
	public int shootableMask;

	Ray bulletMid;
	Ray bulletRight;
	Ray bulletLeft;
	Ray bulletTop;
	Ray bulletDown;
	RaycastHit hit;

	AudioSource gunAudio;
	ParticleSystem gunParticles;
	Camera playerCam;


	// Use this for initialization
	void Start () {
		shootableMask = LayerMask.GetMask ("Shootable");
		gunAudio = GetComponent<AudioSource>();
		gunParticles = GetComponentInChildren<ParticleSystem>();
		playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
	}

	public void AmmoPickup(){
		maxAmmo += 3;
		HUDManager.UpdateAmmo (currentAmmo, maxAmmo);
	}

	public void Reload(){ //function call located in animation behavior; ensures ammo update only after animation finishes
		if(maxAmmo >= 6){
			maxAmmo -= (6 - currentAmmo);
			currentAmmo = 6;
		} else {
			currentAmmo = maxAmmo;
			maxAmmo = 0;
		}
		HUDManager.ToggleCrosshair (true);
		HUDManager.UpdateAmmo (currentAmmo, maxAmmo);
	}

	
	public void Fire(){
		if(currentAmmo > 0){
			currentAmmo -= 1;
			HUDManager.UpdateAmmo (currentAmmo, maxAmmo);
			gunParticles.Stop();
			gunParticles.Play();
			gunAudio.PlayOneShot(fireSound);
			bulletMid = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
			bulletRight = playerCam.ScreenPointToRay(new Vector3((Screen.width / 2)+10, Screen.height / 2, 0));
			bulletLeft = playerCam.ScreenPointToRay(new Vector3((Screen.width / 2)-10, Screen.height / 2, 0));
			bulletTop = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, (Screen.height / 2)+10, 0));
			bulletDown = playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, (Screen.height / 2)-10, 0));
			if ((Physics.Raycast(bulletMid, out hit, 50, shootableMask)) || (Physics.Raycast(bulletRight, out hit, 50, shootableMask)) || 
				(Physics.Raycast(bulletLeft, out hit, 50, shootableMask)) || (Physics.Raycast(bulletTop, out hit, 50, shootableMask)) || 
				(Physics.Raycast(bulletDown, out hit, 50, shootableMask))) {
				Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
				if(enemy != null && enemy.enemyHealth > 0){
					enemy.bloodSplat.gameObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z+1f);
					Vector3 playerPos = transform.position;
					Vector3 enemyPos = enemy.transform.position;
					playerPos.y = 0; 
					enemyPos.y = 0;
					Vector3 direction = playerPos - enemyPos;
					if (hit.collider.tag == "ZombieHead") {
						Debug.Log("Headshot!");
						enemy.TakeDamage(100);
					} else {
						enemy.TakeDamage(gunDamage);
						Debug.Log(hit.collider.name + " got shot!");
					}

					if(enemy.enemyHealth <= 0){
						enemy.Die(Vector3.Dot(direction, enemy.transform.forward) > 0);
					}
				} else {
					Debug.Log (hit.collider.name);
				}
			} else {
				Debug.Log ("GUN FIRES... NOTHING WAS HIT.");
			}
		}
	}
}
