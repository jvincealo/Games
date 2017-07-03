using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour {
	public GameObject player;
	public ObjectPooler TerrainPool;
	float terrainWidth;
	public List<GameObject> currentTerrains;
	public GameObject activeTerrain;
	public GameObject previousTerrain;
	public GameObject[] obstacles;
	public GameObject[] coins;
	public GameObject[] enemies;
	public GameObject[] powerups;
	public GameObject[] debris;


	bool addTerrain;
	float objBoundsX = 6;
	float minObjY = -2.5f;
	float maxObjY = 3.5f;
	float powerupSpawnTimer;
	float powerupSpawnTime = 10;

	void Start () {
		float height = 2.0f * Camera.main.orthographicSize;
		terrainWidth = height * Camera.main.aspect;
		activeTerrain = currentTerrains [0];
		GenerateObjects (currentTerrains [1].transform.position.x);
		GenerateObjects (currentTerrains [2].transform.position.x);
	}

	void Update(){
		powerupSpawnTimer += Time.deltaTime;
	}

	void FixedUpdate () {
		GenerateTerrain (); //faster(?) than collision detection for terrain generation
	}

	void GenerateTerrain(){
		float playerX = player.transform.position.x; //get player's position
		float addTerrainX = activeTerrain.transform.position.x + terrainWidth/4; //x value for 3/4 of current room
		float removeTerrainX = activeTerrain.transform.position.x; //x value for next terrain's center
		float endTerrainX = activeTerrain.transform.position.x + terrainWidth/2; //x value for ending edge of terrain

		if(playerX >= removeTerrainX && previousTerrain != null){ //remove previous terrain, execute before updating active terrain
			currentTerrains.Remove (previousTerrain);
			TerrainPool.DisableTerrain (previousTerrain);
		}
		if(playerX >= endTerrainX){ //update active terrain
			int activeIndex = currentTerrains.IndexOf(activeTerrain);
			previousTerrain = activeTerrain; //sets the current terrain as previous
			activeTerrain = currentTerrains [activeIndex + 1]; //sets the current next terrain as active
			addTerrain = true;
		}
		if(playerX >= addTerrainX && addTerrain){ //generate terrain
			float newTerrainX = currentTerrains[currentTerrains.Count-1].transform.position.x + terrainWidth; //center of new terrain
			currentTerrains.Add(TerrainPool.GetTerrain(newTerrainX)); //place terrain from object pool at the end of active terrain
			GenerateObjects(newTerrainX); //generate coins and obstacles on the new terrain
			GenerateEnemies(newTerrainX);
			GenerateDebris(newTerrainX);
			if (powerupSpawnTimer >= powerupSpawnTime){
				SpawnPowerup (newTerrainX);
				powerupSpawnTimer = 0;
			}
			addTerrain = false;
		}
	}

	void GenerateObjects(float terrainX){
		float currentObjX = 0;
		float currentObjY = 0;
		int obstacleCount = Random.Range (2,4); //number of debris to generate
		int coinCount = Random.Range(1, 4); //number of coins to generate
		List<Vector2> currentObstaclePos = new List<Vector2>();

		//generate obstacles
		for(int i=0; i<obstacleCount; i++){
			if((terrainX + objBoundsX) >= (currentObjX + objBoundsX)){ //if max x value is within terrain bounds
				GameObject newObstacle = Instantiate(obstacles[Random.Range (0, obstacles.Length)]) as GameObject;
				if(i == 0){
					currentObjX = terrainX - objBoundsX; //set the first obstacle at the left most, creation is from left to right
				} else {
					currentObjX = currentObjX + objBoundsX; //set obstacle distance from each other
				}
				newObstacle.transform.position = new Vector2 (currentObjX, Random.Range (minObjY, maxObjY));
				currentObstaclePos.Add (newObstacle.transform.position);
			} else {
				Debug.Log ((terrainX + objBoundsX) + "<" + (currentObjX + objBoundsX) + " OUT OF BOUNDS!");
				break;
			}
		}

		currentObjX = 0;

		//generate coins
		for(int i=0; i<coinCount; i++){
			if((terrainX + objBoundsX) >= (currentObjX + objBoundsX)){ //if max x value is within terrain bounds
				GameObject coin = Instantiate(coins[Random.Range(0, coins.Length)]) as GameObject;
				if(i == 0){
					currentObjX = currentObstaclePos [i].x + objBoundsX / 2; //generate coins a little bit to the right of the obstacles
				} else {
					currentObjX = currentObjX + objBoundsX; //set coin distance from each other, same interval as
				}
				currentObjY = Random.Range (minObjY, maxObjY);
				coin.transform.position = new Vector2 (currentObjX, currentObjY);
			}
		}
	}

	void GenerateEnemies(float terrainX){ 
		GameObject newEnemy = Instantiate(enemies[Random.Range (0, enemies.Length)]) as GameObject;
		newEnemy.transform.position = new Vector2(terrainX + terrainWidth/2, Random.Range (minObjY / 2, maxObjY));
	}

	void GenerateDebris(float terrainX){ 
		GameObject newDebris = Instantiate(debris[Random.Range (0, debris.Length)]) as GameObject;
		newDebris.transform.position = new Vector2(terrainX + terrainWidth/2, Random.Range (minObjY / 2, maxObjY));
	}

	void SpawnPowerup(float terrainX){
		GameObject newPowerup = Instantiate(powerups[Random.Range (0, powerups.Length)]) as GameObject;
		newPowerup.transform.position = new Vector2(terrainX + terrainWidth/2, Random.Range (minObjY / 2, maxObjY));
		powerupSpawnTimer = 0;
		powerupSpawnTime = Random.Range (15, 45); //randomize new spawn time
	}
}
