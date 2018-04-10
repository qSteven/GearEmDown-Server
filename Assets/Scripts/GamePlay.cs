using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
	//status -2 = game resets
	//status -1 = no wave (before pressing "start game")
	//status 0 = after pressing "start game" and after game was lost or entered. endless "wave" without enemies
	public static int status = -1;

	//Counts the rounds.
	public static int round = 0;

	//Will be set true, when "start game" was pressed.
	//Will be set false, after the game was lost
	public static bool gameActive = false;

	//Will be set true, when "start round" was pressed.
	//Will be set false, when the last group in the last wave of the round spawned.
	public static bool roundActive = false;

	public static bool showNextRound = false;

	//Actually "status" and "gameActive / roundActive" are basically the same, but I'm too lazy to adjust this

	// Update is called once per frame
	void Update () {
		if (status == -2) {
			ResetGame ();
		}

		if (!showNextRound && gameActive && !roundActive && (GetEnemies().Length == 0)) {
			showNextRound = true;
			Connection.SendToAll ("1|5");
		}
	}

	//Resets every stat in the game to its base value.
	void ResetGame() {
		status = -1;
		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if (BuildingManager.towersInScene [i] != null) {
				if (BuildingManager.towersInScene [i].CompareTag ("TowerBuff")) {
					Destroy (BuildingManager.towersInScene [i]);
				}
			}
		}

		for (int i = 0; i < BuildingManager.towersInScene.Count; i++) {
			if (BuildingManager.towersInScene [i] != null) {
				Destroy (BuildingManager.towersInScene [i]);
			}
		}
		
		round = 0;
		gameActive = false;
		roundActive = false;
		showNextRound = false;
		Round.CurrentWaveNumber = 0;
		Round.maxEnemies = 15;
		BaseScript.health = 20;
		GameStatsScript.baseHealth = 20;
		GameStatsScript.money = 500;
		GameStatsScript.kills = 0;
		Building.idIncrement = 0;
		EnemyScript.idIncrement = 0;

		if (EnemyScript.EnemiesActive != null) {
			for (int i = 0; i < EnemyScript.EnemiesActive.Length; i++) {
				if (EnemyScript.EnemiesActive [i] != null) {
					Destroy (EnemyScript.EnemiesActive [i]);
					EnemyScript.EnemiesActive [i] = null;
				}
			}
			EnemyScript.EnemiesActive = null;
		}

		EnemySpawnScript.instance.waveCount = 0;
		EnemySpawnScript.instance.round = new Round
			(
				new Wave[]
				{
					new Wave (new List<Group>{
						new Group (3, 0, 0),
						new Group (3, 0, 0),
						new Group (5, 0, 0, 15)
					}),
					new Wave (new List<Group> {
						new Group (5, 0, 0),
						new Group (5, 0, 0),
						new Group (0, 1, 0, 15)
					}),
					new Wave (new List<Group> {
						new Group (3, 0, 0),
						new Group (0, 2, 0),
						new Group (3, 0, 0),
						new Group (0, 2, 0, 15)
					}),
					new Wave (new List<Group> {
						new Group (5, 1, 0),
						new Group (5, 2, 0),
						new Group (5, 0, 0),
						new Group (0, 3, 0, 15)
					}),
					new Wave (new List<Group> {
						new Group (0, 0, 3),
						new Group (8, 0, 0),
						new Group (0, 2, 5),
						new Group (10, 0, 0, 15)
					})
				}
			);
		
		GameStatsScript.UpdateStats ();
		Debug.Log ("reset");
	}

	//Only for checking the remaining enemies, so that they can be deleted in Reset().
	public static GameObject[] GetEnemies() {
		GameObject[] activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		return activeEnemies;
	}
}
