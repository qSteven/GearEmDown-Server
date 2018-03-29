using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
	/// <summary>
	/// MS: V.10.0 Enemy finds his way with help of NavMesh to the base.
	/// </summary>

	//Destination of every enemy:
	private GameObject playerBase;
	private Vector3 basePosition;

	//speed = how fast an enemy moves
	public float speed;

	//lifePoints = how much life an enemy has
	public float lifePoints;

	//damage = how much damage enemies deal to the base
	public int damage;

	//reward = how much money the player gets, when an enemy was destroyed by a tower
	public int reward;

	//Will be initialised every update.
	public static GameObject[] EnemiesActive;

	//Event: When an enemy dies it has to unsubscribe in every tower's list he's in.
	public delegate void EnemyKilled(EnemyScript enemy);
	public event EnemyKilled EnemyKilledEvent = delegate(EnemyScript enemy){};

	NavMeshAgent agent;
	public bool isSlow = false;
	public static int idIncrement;
	public int id;

	public int prefabID;

	// Use this for initialization
	void Start () {
		idIncrement++;
		id = idIncrement;
		agent = gameObject.GetComponent<NavMeshAgent> ();
		agent.speed = speed;
		playerBase = GameObject.Find ("Base");
		basePosition = playerBase.transform.position;

		Connection.SendToAll ("6|1;" + prefabID + ";" + id);

		//Yo dude
		//NavMesh is pretty shitty because you can't let an agent fly above other agents lmao
		//So I created another NavMesh for the flying enemies.
		//But it's buggy because the flying enemies had problems with its destination.
		//And i couldn't change the tag of the enemies because it's "Enemy" everywhere and I already use this.
		//So I used this cheesy solution, lol, don't laugh, it's working:
		if(gameObject.GetComponent<NavMeshAgent>().baseOffset == 0.2f)
			agent.SetDestination (new Vector3(basePosition.x, basePosition.y+1, basePosition.z));
		else
			agent.SetDestination (basePosition);
	}
	
	// Update is called once per frame
	void Update () {
		EnemiesActive = GamePlay.GetEnemies ();
	}

	//Called by towers, when they attack an enemy.
	//If enemy reaches 0 lifePoints, reward will be added to player money and the kill counter increases.
	//Of course gameObject is getting destroyed too, lol.
	public void TakeDamage(float damage){
		if (GamePlay.gameActive) {
			if (gameObject != null) {
				lifePoints -= damage;
				if ((lifePoints <= 0) && (gameObject != null)) {
					GameStatsScript.money += reward;
					GameStatsScript.UpdateKills ();
					Destroy (gameObject);
				}
			}
		}
	}

	//Event
	void OnDestroy()
	{
		EnemyKilledEvent(this);
		if (Connection.connectionList.Count > 0) Connection.SendToAll((int)ProtocolDef.Enemy + "|3;" + id);
	}
}
