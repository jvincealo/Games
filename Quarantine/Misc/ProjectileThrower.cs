using UnityEngine;
using System.Collections;

public class ProjectileThrower : MonoBehaviour {
	GameObject player;
	public GameObject projectile;
	public float initialVelocity = 10f;
	public float timeResolution = 0.02f;
	public float maxTime = 10f;

	bool readyThrow;
	bool throwRelease;
	LayerMask mask;

	LineRenderer lineRenderer;

	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
		player = GameObject.FindGameObjectWithTag ("Player");
		mask = LayerMask.GetMask ("Shootable", "Default");
	}

	void Update(){
		if (throwRelease) {
			GameObject item = Instantiate (projectile, transform.position, transform.localRotation) as GameObject;
			item.GetComponent<Rigidbody> ().AddForce (transform.forward * initialVelocity/Time.fixedDeltaTime);
			lineRenderer.SetVertexCount (0);
			throwRelease = false;
			readyThrow = false;
		}
	}

	void FixedUpdate () {
		if(readyThrow){
			Vector3 velocityVector = transform.forward*initialVelocity;
			lineRenderer.SetVertexCount((int)(maxTime/timeResolution)); //number of vertices to create curves

			int index = 0; //current vertex
			Vector3 currentPosition = transform.position;

			for(float t = 0f; t < maxTime; t+= timeResolution){
				lineRenderer.SetPosition(index, currentPosition); //create line segments continously to form curve line
				Debug.DrawRay (currentPosition, velocityVector*timeResolution, Color.red);
				RaycastHit hit; //raycast point of impact
				if (Physics.Raycast (currentPosition, velocityVector, out hit, velocityVector.magnitude*timeResolution,mask)) {
					if(hit.collider.gameObject != player){
						currentPosition += velocityVector*timeResolution;
						velocityVector += Physics.gravity*timeResolution;
						index++;
						lineRenderer.SetVertexCount(index+1);
						lineRenderer.SetPosition(index, currentPosition);
						break;
					}
				}
				currentPosition += velocityVector*timeResolution;
				velocityVector += Physics.gravity*timeResolution;
				index++;
			}
		}
	}

	public void setReadyThrow(bool value){
		readyThrow = value;
		if (!value)
			lineRenderer.SetVertexCount (0);
	}

	public void cancelThrow(){
		throwRelease = false;
	}

	public void ThrowProjectile(){
		throwRelease = true;
	}
}
