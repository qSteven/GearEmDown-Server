using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TowerSlowScript : MonoBehaviour {

	/// <summary>
	/// MS: V.10.0 This tower is slowing enemies which are in its range.
	/// Enemies which enter the tower's collider will be added to the list below.
	/// 
	/// Slow multiplier can't stack on enemies!
	/// Only the multiplier of the tower which an enemy entered first will apply.
	/// </summary>
	private List<GameObject> enemyList = new List<GameObject>();

	//Checks every update if an enemy which isn't slow already is in range.
	//This happens, when an enemy leaves another slow tower's collider and is already in a next collider. (Game won't notice because only OnTriggerEnter and OnTriggerExit are used)
	void Update(){
		foreach (GameObject enemy in enemyList) {
			if(enemy != null) if (!enemy.gameObject.GetComponent<EnemyScript> ().isSlow) {
				enemy.gameObject.GetComponent<EnemyScript> ().isSlow = true;
				enemy.gameObject.GetComponent<NavMeshAgent> ().speed *= gameObject.GetComponent<Building> ().slowMultiplier;
			}
		}
	}

	//If an enemy enters the collider of this tower, it will be slowed.
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Enemy")) {
			enemyList.Add (other.gameObject);
			if (!other.gameObject.GetComponent<EnemyScript> ().isSlow) {
				other.gameObject.GetComponent<EnemyScript> ().isSlow = true;
				other.gameObject.GetComponent<NavMeshAgent> ().speed *= gameObject.GetComponent<Building>().slowMultiplier;
			}
		}
	}

	//Adjusting enemies' speed, if you upgrade a slow tower while enemies are already in its collider.
	public void UpdateSlow(float newSlow){
		for (int i = 0; i < enemyList.Count; i++) {
			enemyList[i].gameObject.GetComponent<NavMeshAgent> ().speed /= gameObject.GetComponent<Building>().slowMultiplier;
			enemyList[i].gameObject.GetComponent<NavMeshAgent> ().speed *= gameObject.GetComponent<Building> ().slowMultiplier - newSlow;
		}
		gameObject.GetComponent<Building> ().slowMultiplier -= newSlow;
	}

	//Event: If an enemy was killed he removes himself from the tower's list.
	void OnEnemyKilled(EnemyScript enemy)
	{
		enemyList.Remove (enemy.gameObject);
	}

	//If he leaves the collider again, its speed is set to normal again.
	void OnTriggerExit(Collider other){
		if (other.CompareTag ("Enemy")) {
			if (other.gameObject.GetComponent<EnemyScript> ().isSlow) {
				enemyList.Remove (other.gameObject);
				other.gameObject.GetComponent<EnemyScript> ().isSlow = false;
				other.gameObject.GetComponent<NavMeshAgent> ().speed /= gameObject.GetComponent<Building>().slowMultiplier;
			}
		}
	}

	//When this tower is sold, enemies in his range will gain back their old velocity.
	void OnDestroy(){
		for (int i = 0; i < enemyList.Count; i++) {
			if (enemyList [i] != null) {
				if (enemyList [i].GetComponent<EnemyScript> ().isSlow) {
					enemyList [i].GetComponent<EnemyScript> ().isSlow = false;
					enemyList [i].GetComponent<NavMeshAgent> ().speed /= gameObject.GetComponent<Building> ().slowMultiplier;
				}
			}
		}
		BuildingManager.towersInScene.Remove (gameObject);
	}
}
