using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
	public static EnemySpawnScript instance;

	//e = nArrow
	//e2 = Steamtank
	//e3 = AirShip
	public GameObject e;
	public GameObject e2;
	public GameObject e3;

	//Will be generated new, every time a round ended.
	public Round round;

	//Position where the enemies spawn.
	private Vector3 spawnPosition;

	public int waveCount;

	void Awake() {
		//Singelton
		instance = this;

		//First round is custom.
		round = new Round
		(
			new Wave[]
			{
				new Wave (new List<Group>{
					new Group (3, 0, 0),
					new Group (3, 0, 0),
					new Group (5, 0, 0, 20)
				}),
				new Wave (new List<Group> {
					new Group (5, 0, 0),
					new Group (5, 0, 0),
					new Group (0, 1, 0, 20)
				}),
				new Wave (new List<Group> {
					new Group (3, 0, 0),
					new Group (0, 2, 0),
					new Group (3, 0, 0),
					new Group (0, 2, 0, 20)
				}),
				new Wave (new List<Group> {
					new Group (5, 1, 0),
					new Group (5, 2, 0),
					new Group (5, 0, 0),
					new Group (0, 3, 0, 20)
				}),
				new Wave (new List<Group> {
					new Group (0, 0, 3),
					new Group (8, 0, 0),
					new Group (0, 2, 5),
					new Group (10, 0, 0, 20)
				})
			}
		);
	}

	// Use this for initialization
	void Start() {
		//Script is located at the EnemySpawn GameObject.
		spawnPosition = transform.position;
	}

	void Update() {
		//Stops coroutine if the game has ended.
		if (!GamePlay.gameActive) StopCoroutine ("SpawnEnemies");
	}

	public void StartRound() {
		//Starts coroutine when this function was called and the game is active.
		if(GamePlay.gameActive) StartCoroutine ("SpawnEnemies");
	}

	//Spawn enemies. Rounds have waves and waves have groups.
	//This method calls the next group and stops, after a round is over.
	//Between waves is a higher spawn delay than between groups.
	IEnumerator SpawnEnemies() {
		Group g = round.NextGroup ();

		if (g != null) {
			//Debug.Log ("Group: " + g.enemyAmount + "----------" + g.enemyAmount2 + "----------" + g.enemyAmount3);
			for (int i = 0; i < g.enemyAmount; i++) {
				GameObject enemy1 = Instantiate (e, spawnPosition, Quaternion.identity) as GameObject;
				enemy1.transform.rotation = Quaternion.Euler (0, 180, 0);
			}
			for (int i = 0; i < g.enemyAmount2; i++) {
				GameObject enemy2 = Instantiate (e2, spawnPosition, Quaternion.identity) as GameObject;
				enemy2.transform.rotation = Quaternion.Euler (0, 180, 0);
			}
			for (int i = 0; i < g.enemyAmount3; i++) {
				GameObject enemy3 = Instantiate (e3, new Vector3(spawnPosition.x, 1f, spawnPosition.z), Quaternion.identity) as GameObject;
				enemy3.transform.rotation = Quaternion.Euler (0, 180, 0);
			}
			yield return new WaitForSeconds (g.groupDelay);
			StartCoroutine ("SpawnEnemies");
		}
		//If a round has ended, the max amount of enemies in a wave will be increased.
		else {
			Debug.Log ("END ROUND");
			GamePlay.roundActive = false;
			round = new Round ();
			Round.maxEnemies += 10;
			GameStatsScript.UpdateStats ();
		}
	}
}