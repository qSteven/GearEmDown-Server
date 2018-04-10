using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargetScript : MonoBehaviour
{
	/// <summary>
	/// MS: V.10.0 This tower attacks the first enemy which entered the tower's collider until it leaves it.
	/// Every enemy which enters the tower's collider will be added to a list (enemyPriority) and TakeDamage(dmg) will be called on the first enemy in this list.
	/// </summary>

	//These variables are necessary for calling the coroutine in which the tower shoots at enemies.
	private bool isShooting = false;
	private bool shot = false;

	//This is the list I mentioned above.
	private List<GameObject> enemyPriority = new List<GameObject>();
	
	// Update is called once per frame
	void Update () {
		if (enemyPriority.Count == 0) {
			isShooting = false;
		}

		if (isShooting) {
			if (!shot) {
				shot = true;
				StartCoroutine ("DamageEnemy", enemyPriority [0]);
			}
		}
	}

	//The enemy enters the collider and is saved as "other".
	//The enemy is added to the tower's list.
	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Enemy")) {
			enemyPriority.Add (other.gameObject);
			other.GetComponent<EnemyScript>().EnemyKilledEvent += OnEnemyKilled;
			isShooting = true;
		}
	}

	//Event: If an enemy was killed he removes himself from the tower's list.
	void OnEnemyKilled(EnemyScript enemy)
	{
		enemyPriority.Remove (enemy.gameObject);
	}

	//The enemy leaves the collider and is saved as "other".
	//He will be removed from the tower's list.
	void OnTriggerExit(Collider other) {
		if (other.CompareTag ("Enemy")) {
			enemyPriority.Remove(other.gameObject);
			other.GetComponent<EnemyScript>().EnemyKilledEvent -= OnEnemyKilled;
		}
	}

	//This is the coroutine which is called when the enemy is being shot at.
	//TakeDamage(int dmg) which is a method at every enemy will be called by the tower.
	//The tower calls this method only at the first enemy in his list (enemyPriority[0] - given by update, saved as "enemy" here).
	IEnumerator DamageEnemy(GameObject enemy) {
			if (enemy.GetComponent<EnemyScript>() != null) {
			enemy.GetComponent<EnemyScript> ().TakeDamage (gameObject.GetComponent<Building>().damage);
			Connection.SendToAll ("6|2;" + gameObject.GetComponent<Building>().id + ";" + enemy.GetComponent<EnemyScript>().id + ";" + (enemy.GetComponent<EnemyScript>().lifePoints - gameObject.GetComponent<Building> ().damage));
			//Debug.Log ("6|2;" + gameObject.GetComponent<Building>().id + ";" + enemy.GetComponent<EnemyScript>().id + ";" + (enemy.GetComponent<EnemyScript>().lifePoints - gameObject.GetComponent<Building> ().damage));

				if(enemy.GetComponent<EnemyScript>() != null) {
				if(gameObject.GetComponent<Building>().fireRate > 0.1f)
					Debug.DrawLine (new Vector3(transform.position.x, transform.position.y+3.7f, transform.position.z), enemy.transform.position, Color.red, 0.1f);
				else
					Debug.DrawLine (new Vector3(transform.position.x, transform.position.y+2.9f, transform.position.z), enemy.transform.position, Color.red, 0.1f);
				}
			yield return new WaitForSeconds (gameObject.GetComponent<Building>().fireRate);
		}
		shot = false;
	}

	//Event: Unsubscribes from the event when being sold.
	void OnDestroy(){
		BuildingManager.towersInScene.Remove (gameObject);
		for (int i = 0; i < enemyPriority.Count; i++) {
			enemyPriority[i].GetComponent<EnemyScript>().EnemyKilledEvent -= OnEnemyKilled;
		}
	}
}